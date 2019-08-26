
using System;
using System.Net.Sockets;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;

namespace Sers.Mq.Socket.ThreadWait
{
    public class ClientMq: IClientMq
    {
        /// <summary>
        /// 连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        /// </summary>
        public string secretKey { get; set; }


        MqConn _mqConn = new MqConn();
 

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> OnGetFrame { set { _mqConn.OnGetFrame = value; } get => _mqConn.OnGetFrame; }

        public Action<IMqConn> Conn_OnDisconnected { get=> _mqConn.Conn_OnDisconnected; set=> _mqConn.Conn_OnDisconnected=value; }

   
        public IMqConn mqConn => _mqConn;


        /// <summary>
        ///  Mq 服务端 host地址。例如： "127.0.0.1"、"sersms.com"。(appsettings.json :: Sers.Mq.Config.ClientMqBuilder[x].host)
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// Mq 服务端 监听端口号。例如： 10345。(appsettings.json :: Sers.Mq.Config.ClientMqBuilder[x].port)
        /// </summary>
        public int port = 10345;


        public bool Connect()
        {
            Logger.Info("[ClientMq] Socket.ThreadWait,connecting... host:" + host + " port:" + port);
            TcpClient client = null;
            try
            {
                client = new System.Net.Sockets.TcpClient(host, port);
            }
            catch (Exception ex)
            {
                //服务启动失败
                Logger.Error("[ClientMq] Socket.ThreadWait,connect - Error", ex);
                return false;
            }
          
  

            _mqConn.Init(client);

            _mqConn.StartBackThreadToReceiveMsg();
            Logger.Info("[ClientMq] Socket.ThreadWait,connected.");
            return true;
        }

        /// Disconnect from the host.
        public void Close()
        {
            try
            {
                _mqConn.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            _mqConn = null;

        }
      
 


 




       








    }
}
