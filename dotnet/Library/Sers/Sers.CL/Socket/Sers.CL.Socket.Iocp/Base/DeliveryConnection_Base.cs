using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.CL.Socket.Iocp.Base
{
    public abstract class DeliveryConnection_Base : IDeliveryConnection
    {

        public SocketAsyncEventArgs receiveEventArgs;


        protected Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;
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
        protected DateTime connectTime { get; set; }






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

        PipeFrame pipe = new PipeFrame() { OnDequeueData = ArraySegmentBytePool_Extensions.ReturnToPool };

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




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SendFrameAsync(ByteData data);
    }
}
