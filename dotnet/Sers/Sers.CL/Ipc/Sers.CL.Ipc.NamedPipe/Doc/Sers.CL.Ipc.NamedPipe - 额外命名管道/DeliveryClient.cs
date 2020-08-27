using System;
using System.IO.Pipes;
using System.Net.Sockets;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;

namespace Sers.CL.Ipc.NamedPipe
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






        public string serverName = ".";
        public string pipeName = "demo";


        string connKey;

        public bool Connect()
        {
            Logger.Info("[CL.DeliveryClient] Socket.ThreadWait,connecting... serverName:" + serverName + " pipeName:" + pipeName);
     
            try
            {
                connKey=ConnectionKeyHelp.Subscribe(pipeName, serverName);          
                
            }
            catch (Exception ex)
            {
                //服务启动失败
                Logger.Error("[CL.DeliveryClient] Socket.ThreadWait,connect - Error", ex);
                return false;
            }


            NamedPipeClientStream client = new NamedPipeClientStream(serverName, pipeName + "." + connKey, PipeDirection.InOut, PipeOptions.Asynchronous);

            //var task=client.ConnectAsync();

            //task.Wait(10000);

            //if (!task.IsCompleted)
            //{
            //    return false;
            //}

            client.Connect(10000);

            if (!client.IsConnected) 
            {
                return false;
            }

            Logger.Info("[Ipc]客户端已创建，connKey：" + connKey);

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
