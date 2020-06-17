using System;
using System.IO.Pipes;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;

namespace Sers.CL.Ipc.NamedPipe
{
    public class DeliveryClient: IDeliveryClient
    {

        /// <summary>
        /// 尝试连接超时时间（单位：毫秒）（默认：1000）
        /// </summary>
        public int connectTimeoutMs = 1000;

        DeliveryConnection _conn  = new DeliveryConnection();
        public IDeliveryConnection conn => _conn;

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。接收到客户端的数据事件
        /// </summary>
        public Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set { _conn.OnGetFrame = value; } }

        public Action<IDeliveryConnection> Conn_OnDisconnected { set=> _conn.Conn_OnDisconnected=value; }


        /// <summary>
        /// 服务端机器名或者ip地址（如 103.23.23.23 、win10f），默认 "."
        /// </summary>
        public string serverName = ".";
        /// <summary>
        /// 如： "Sers.CL.Ipc"
        /// </summary>
        public string pipeName = "Sers.CL.Ipc";
 

        public bool Connect()
        {
            Logger.Info("[CL.DeliveryClient] Ipc.NamedPipe, connecting... serverName:" + serverName + " pipeName:" + pipeName);

            NamedPipeClientStream client;
            try
            {

                client = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

                client.Connect(connectTimeoutMs);

                if (!client.IsConnected)
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                //服务启动失败
                Logger.Error("[CL.DeliveryClient] Ipc.NamedPipe, connect - Error", ex);
                return false;
            }
             

            _conn.Init(client);

            _conn.StartBackThreadToReceiveMsg();
            Logger.Info("[CL.DeliveryClient] Ipc.NamedPipe, connected.");
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
