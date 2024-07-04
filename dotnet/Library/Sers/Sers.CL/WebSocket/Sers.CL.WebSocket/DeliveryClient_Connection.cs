using System;
using System.Net.WebSockets;
using System.Threading;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Core.Util.Threading.Worker;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.CL.WebSocket
{
    public class DeliveryClient_Connection : IDeliveryConnection
    {
        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _securityManager = value; }
        Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { internal get; set; }


        public Action<IDeliveryConnection> Conn_OnDisconnected { get; set; }

        public void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            if (data == null || socket == null) return;
            try
            {

                Int32 len = data.Count();
                data.Insert(0, len.Int32ToArraySegmentByte());

                var bytes = data.ToBytes();

                _securityManager?.Encryption(new ArraySegment<byte>(bytes, 4, bytes.Length - 4));

                socket.SendAsync(bytes.BytesToArraySegmentByte(), WebSocketMessageType.Binary, true, _cancellation)
                    .GetAwaiter().GetResult()
                    ; //发送数据     
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Close();
            }

        }

        CancellationToken _cancellation = new CancellationToken();
        public void Close()
        {
            if (socket == null) return;


            state = DeliveryConnState.closed;

            var socket_ = socket;
            socket = null;

            try
            {
                socket_.CloseAsync(WebSocketCloseStatus.NormalClosure, "", _cancellation).GetAwaiter().GetResult();
                //socket_.Abort();

                socket_.Dispose();
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
        public void Init(ClientWebSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// 通信SOCKET
        /// </summary>
        public ClientWebSocket socket { get; private set; }



        PipeFrame pipe = new PipeFrame();

        private void AppendData(ArraySegment<byte> data)
        {
            pipe.Write(data);

            while (pipe.TryRead_SersFile(out var msgFrame))
            {
                _securityManager?.Decryption(msgFrame);

                OnGetFrame.Invoke(this, msgFrame);
            }
        }

        #region taskToReceiveMsg       

        LongThread taskToReceiveMsg = new LongThread();
        public void StartBackThreadToReceiveMsg()
        {
            taskToReceiveMsg.Stop();

            taskToReceiveMsg.threadName = "Sers.CL.WebSocket-taskToReceiveMsg";
            taskToReceiveMsg.threadCount = 1;
            taskToReceiveMsg.Processor = TaskToReceiveMsg;
            taskToReceiveMsg.Start();
        }
        /// <summary>
        /// 缓存区大小
        /// </summary>
        public int receiveBufferSize = 8 * 1024;
        void TaskToReceiveMsg()
        {
            while (socket != null)
            {
                try
                {
                    while (socket != null)
                    {
                        var data = new byte[receiveBufferSize];
                        var result = socket.ReceiveAsync(new ArraySegment<byte>(data), _cancellation).GetAwaiter().GetResult();
                        AppendData(new ArraySegment<byte>(data, 0, result.Count));
                    }
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                    Close();
                    return;
                }
            }
        }
        #endregion


    }
}
