// https://blog.csdn.net/Qin066/article/details/84971890

using System;
using System.Net.WebSockets;
using System.Threading;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;

namespace Sers.CL.WebSocket
{
    public class DeliveryClient: IDeliveryClient
    {


        DeliveryClient_Connection _conn = new DeliveryClient_Connection();
        public IDeliveryConnection conn => _conn;

        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set { _conn.OnGetFrame = value; }  }


        public Action<IDeliveryConnection> Conn_OnDisconnected { set => _conn.Conn_OnDisconnected = value; }


        /// <summary>
        /// 服务端地址(默认为 "ws://127.0.0.1:4503")
        /// </summary>
        public string host = "ws://127.0.0.1:4503";


        public bool Connect()
        {
            try
            {
                Logger.Info("[CL.DeliveryClient] WebSocket,connecting... host:" + host);

                ClientWebSocket _webSocket = new ClientWebSocket();
                _webSocket.ConnectAsync(new Uri(host), new CancellationToken()).GetAwaiter().GetResult();
                _conn.Init(_webSocket);                

                _conn.StartBackThreadToReceiveMsg();

                Logger.Info("[CL.DeliveryClient] WebSocket,connected.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

    
        public void Close()
        {
            if (null == _conn) return;
            var conn = _conn;
            _conn = null;
            conn.Close();
        }
 
 
 




    }
}
