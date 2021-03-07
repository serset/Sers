using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Core.Util.Pool;
using Vit.Extensions;

namespace Sers.CL.Ipc.NamedPipe
{
    /// <summary>
    ///  
    /// </summary>
    public class DeliveryConnection : IDeliveryConnection
    {

        ~DeliveryConnection()
        {
            Close();
        }

        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _securityManager = value; }
        Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;


        public Action<IDeliveryConnection> Conn_OnDisconnected { get; set; }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { private get; set; }


        public void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            if (data == null || stream == null) return;

            if (!stream.IsConnected) 
            {
                Task.Run((Action)Close);
                return;
            }
            try
            {
                Int32 len = data.Count();
                data.Insert(0, len.Int32ToArraySegmentByte());               

                var bytes = data.ToBytes();
                _securityManager?.Encryption(new ArraySegment<byte>(bytes,4, bytes.Length-4));

                stream.WriteAsync(bytes, 0, bytes.Length);
                stream.FlushAsync();
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                Logger.Error(ex);
                Task.Run((Action)Close);
            }
        }



        public void Close()
        {
            if (state == DeliveryConnState.closed) return;
            state = DeliveryConnState.closed;

            try
            {
                stream.Close();
                stream.Dispose();

                //stream.Shutdown(SocketShutdown.Both);
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


        public void Init(PipeStream stream)
        {
            this.stream = stream;

            connectTime = DateTime.Now;
        }



        #region taskToReceiveMsg       

        class StreamReader
        {
            public PipeStream stream;


            public Action<ArraySegment<byte>> OnReadData;
            public Action OnClose;

            //定义异步读取状态类
            class AsyncState
            {
                public PipeStream stream { get; set; }
                public byte[] buffer { get; set; }        
            }

            /// <summary>
            /// 缓存区大小
            /// </summary>
            public int receiveBufferSize = 8 * 1024;

            public void Start()
            {
                try
                {
                    var buffer = DataPool.BytesGet(receiveBufferSize);

                    var asyncState = new AsyncState { stream = stream, buffer = buffer };

                    //异步读取
                    if (stream.IsConnected)
                    {
                        var result=stream.BeginRead(buffer, 0, receiveBufferSize, new AsyncCallback(AsyncReadCallback), asyncState);                  
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                Task.Run(OnClose);
            }


            //异步读取回调处理方法
            void AsyncReadCallback(IAsyncResult asyncResult)
            {
                try
                {
                    var asyncState = (AsyncState)asyncResult.AsyncState;
                    int readCount = asyncState.stream.EndRead(asyncResult);

                    if (readCount > 0)
                    {
                        //输出读取内容值
                        OnReadData(new ArraySegment<byte>(asyncState.buffer, 0, readCount));

                        asyncState.buffer = DataPool.BytesGet(receiveBufferSize);


                        //再次执行异步读取操作
                        if (asyncState.stream.IsConnected)
                        {
                            asyncState.stream.BeginRead(asyncState.buffer, 0, receiveBufferSize, new AsyncCallback(AsyncReadCallback), asyncState);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                Task.Run(OnClose);
            }
        }


        PipeFrame pipe = new PipeFrame() { OnDequeueData = ArraySegmentByteExtensions.ReturnToPool };

        public void AppendData(ArraySegment<byte> data)
        {
            pipe.Write(data);

            while (pipe.TryRead_SersFile(out var msgFrame))
            {
                _securityManager?.Decryption(msgFrame);
                OnGetFrame.Invoke(this, msgFrame);
            }
        }
        public void StartBackThreadToReceiveMsg()
        {
            new StreamReader
            {
                stream = stream,
                OnReadData = AppendData,
                OnClose=Close
            }.Start();
        }

 
        #endregion



        /// <summary>
        /// 通信对象
        /// </summary>
        public PipeStream stream { get; private set; }



        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime connectTime { get; private set; }


    }
}
