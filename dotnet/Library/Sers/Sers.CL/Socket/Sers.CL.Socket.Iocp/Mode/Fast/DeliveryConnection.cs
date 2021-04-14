using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Extensions;

namespace Sers.CL.Socket.Iocp.Mode.Fast
{
    public class DeliveryConnection : IDeliveryConnection
    {

        public SocketAsyncEventArgs receiveEventArgs;


        Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;
        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _securityManager = value; }
      

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;


        /// <summary>
        /// 通信SOCKET
        /// </summary>
        public global::System.Net.Sockets.Socket socket { get; private set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        private DateTime connectTime { get; set; }


 



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { private get; set; }


        public Action<IDeliveryConnection> Conn_OnDisconnected { get; set; }


        public void Init(global::System.Net.Sockets.Socket socket)
        {
            this.socket = socket;
            connectTime = DateTime.Now;
        }

        public void Close()
        {
            if (socket == null) return;


            state = DeliveryConnState.closed;

            var socket_ = socket;
            socket = null;

          

            try
            {
                socket_.Close();
                socket_.Dispose();

                //socket_.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                Conn_OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }


     



        #region AppendData        

        PipeFrame pipe = new PipeFrame() { OnDequeueData = ArraySegmentByteExtensions.ReturnToPool };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendData(ArraySegment<byte> data)
        {            
            pipe.Write(data);

            while (pipe.TryRead_SersFile(out var msgFrame))
            {
                _securityManager?.Decryption(msgFrame);
                OnGetFrame.Invoke(this, msgFrame);
            }
        }
        #endregion



        #region Send

        ConcurrentQueue<ByteData> queue = new ConcurrentQueue<ByteData>();


        const int buffLength = 1000;
        ByteData[] list = new ByteData[buffLength];
        int[] count = new int[buffLength];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            queue.Enqueue(data);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            int curIndex;
             
            try
            {
                while (true)
                {
                    curIndex = 0;
                    while (true)
                    {
                        if (queue.TryDequeue(out var item))
                        {
                            list[curIndex++] = item;

                            if (curIndex == buffLength)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (curIndex == 0) return;
                            break;
                        }
                    }
                    var bytes = ByteDataArrayToBytes(list, curIndex);
                    try
                    {
                        socket.SendAsync(bytes.BytesToArraySegmentByte(), SocketFlags.None);
                        //socket.SendAsync(data, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Close();
                    }

                    if (curIndex < buffLength)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe byte[] ByteDataArrayToBytes(ByteData[] byteDataArray, int arrayCount)
        {
            //(x.1)get count
            int sumCount = 0;

            int curCount;
            int arrayIndex;
            for (arrayIndex = 0; arrayIndex < arrayCount; arrayIndex++)
            {
                var byteData = byteDataArray[arrayIndex];
                curCount = 0;
                foreach (var item in byteData.byteArrayList)
                {
                    curCount += item.Count;
                }
                count[arrayIndex] = curCount;
                sumCount += curCount;
            }


            //(x.2)copy data
            var bytes = new byte[sumCount + arrayCount * 4];
            arrayIndex = 0;
            curCount = 0;

            int curLength;

            fixed (byte* pTarget = bytes)
            {
                for (arrayIndex = 0; arrayIndex < arrayCount; arrayIndex++)
                {
                    var byteData = byteDataArray[arrayIndex];
                    ((int*)(pTarget + curCount))[0] = curLength = count[arrayIndex];
                    curCount += 4;

                    foreach (var item in byteData.byteArrayList)
                    {
                        if (null == item.Array || item.Count == 0) continue;
                        fixed (byte* pSource = item.Array)
                        {
                            Buffer.MemoryCopy(pSource + item.Offset, pTarget + curCount, item.Count, item.Count);
                        }
                    }
                    _securityManager?.Encryption(new ArraySegment<byte>(bytes, curCount, curLength));
                    curCount += curLength;
                }
            }
            return bytes;
        }

        #endregion

    }
}
