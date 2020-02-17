using System;
using System.Collections.Generic;
using Fleck;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;
using Vit.Extensions;

namespace Sers.CL.WebSocket
{
    public class DeliveryServer_Connection : IDeliveryConnection
    {      
 

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;



        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { internal get; set; }


        public Action<IDeliveryConnection> Conn_OnDisconnected { get; set; }

        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            if (data == null || socket == null) return;

            try
            {
                Int32 len = data.ByteDataCount();
                data.Insert(0, len.Int32ToArraySegmentByte());

                socket.Send(data.ByteDataToBytes());
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
        public void Init(IWebSocketConnection socket)
        {
            this.socket = socket;          
        }
 
        /// <summary>
        /// 通信SOCKET
        /// </summary>
        public IWebSocketConnection socket { get;private set; }



        PipeFrame pipe = new PipeFrame();

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
