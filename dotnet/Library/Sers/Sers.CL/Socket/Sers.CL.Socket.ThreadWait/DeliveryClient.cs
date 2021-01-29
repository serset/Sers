using System;
using System.Net.Sockets;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;

namespace Sers.CL.Socket.ThreadWait
{
    public class DeliveryClient: IDeliveryClient
    {

        DeliveryConnection _conn  = new DeliveryConnection();
        public IDeliveryConnection conn => _conn;


        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set { _conn.OnGetFrame = value; } }

        public Action<IDeliveryConnection> Conn_OnDisconnected { set=> _conn.Conn_OnDisconnected=value; }





        /// <summary>
        /// 服务端 host地址(默认 "127.0.0.1")。例如： "127.0.0.1"、"sers.com"。
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// 服务端 监听端口号（默认 4501）。例如： 4501。
        /// </summary>
        public int port = 4501;


        public bool Connect()
        {
            Logger.Info("[CL.DeliveryClient] Socket.ThreadWait,connecting... host:" + host + " port:" + port);
            TcpClient client = null;
            try
            {
                client = new System.Net.Sockets.TcpClient(host, port);
            }
            catch (Exception ex)
            {
                //服务启动失败
                Logger.Error("[CL.DeliveryClient] Socket.ThreadWait,connect - Error", ex);
                return false;
            }

            _conn.Init(client);

            _conn.StartBackThreadToReceiveMsg();
            Logger.Info("[CL.DeliveryClient] Socket.ThreadWait,connected.");
            return true;
        }

        /// Disconnect from the host.
        public void Close()
        {
            try
            {
                _conn?.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            _conn = null;

        }


    }
}
