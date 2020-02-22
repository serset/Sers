using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Extensions;

namespace Sers.CL.Socket.Iocp
{
    public class DeliveryConnection : IDeliveryConnection
    {

        public SocketAsyncEventArgs receiveEventArgs;
 

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { private get; set; }


        public Action<IDeliveryConnection> Conn_OnDisconnected { get; set; }

        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            if (data == null || socket == null) return;
            try
            {
                Int32 len = data.ByteDataCount();
                data.Insert(0, len.Int32ToArraySegmentByte());

                socket.SendAsync(data, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Close();
            }           
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
        public void Init(global::System.Net.Sockets.Socket socket)
        {
            this.socket = socket;
            connectTime = DateTime.Now;  
        }
 
        /// <summary>
        /// 通信SOCKET
        /// </summary>
        public global::System.Net.Sockets.Socket socket { get;private set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        private DateTime connectTime { get; set; }




        PipeFrame pipe = new PipeFrame() {  OnDequeueData= ArraySegmentByteExtensions.ReturnToPool };

        public void AppendData(ArraySegment<byte> data)
        {
            pipe.Write(data);

            while (pipe.TryRead_SersFile(out var msgFrame))
            {
                OnGetFrame.Invoke(this, msgFrame);
            }
        }


    }
}
