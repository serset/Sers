using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using Vit.Core.Util.Net;
using Vit.Core.Util.Threading;

namespace Sers.CL.Ipc.NamedPipe
{
    public class DeliveryServer: IDeliveryServer
    {

     
        public string pipeName = "demo";


        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; }
                           



 


        /// <summary>
        ///  connHashCode -> DeliveryConnection
        /// </summary>
        readonly ConcurrentDictionary<int, DeliveryConnection> connMap = new ConcurrentDictionary<int, DeliveryConnection>();

        public IEnumerable<IDeliveryConnection> ConnectedList => connMap.Values.Select(conn => ((IDeliveryConnection)conn));


        LongTaskHelp tcpListenerAccept_BackThread = new LongTaskHelp();


        #region Start


        string BeforeConnect() 
        {
            string connKey = CommonHelp.NewGuid();

            Task.Run(() => {


                var server = new NamedPipeServerStream(pipeName + "." + connKey, PipeDirection.InOut);
                //TODO  10秒无连接 强制关闭
                // 等待客户端的连接
                server.WaitForConnection();

                if(server.IsConnected)
                Delivery_OnConnected(server);
                //server.Close();

            });
            return connKey;
        }


        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            try
            {
                

                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,starting... pipeName:" + pipeName);
 


                #region (x.2)启动Task监听listener
                tcpListenerAccept_BackThread.action = () =>
                {
                    try
                    {
                        while (true)
                        {
                            ConnectionKeyHelp.Publish(BeforeConnect, pipeName);
                        }
                    }
                    catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                    {
                        Logger.Error(ex);
                    }
                    finally
                    {
                        Stop();
                    }
                };
                tcpListenerAccept_BackThread.Start();
                #endregion               

                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,started.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;         

        }

        #endregion


        #region Stop

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
     

            //(x.1) stop conn
            ConnectedList.ToList().ForEach(Delivery_OnDisconnected);            
            connMap.Clear();

            //(x.2) close socket
            Task.Run(() =>
            {
                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,stop...");       

                tcpListenerAccept_BackThread.Stop();
                 

                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,stoped");

            });

        }
        #endregion


        #region Delivery_Event


        private DeliveryConnection Delivery_OnConnected(Stream client)
        {
            var conn = new DeliveryConnection();
            conn.Init(client);
        
            conn.Conn_OnDisconnected = Delivery_OnDisconnected; 
            connMap[conn.GetHashCode()] = conn;
            try
            {
                Conn_OnConnected?.Invoke(conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            conn.StartBackThreadToReceiveMsg();

            return conn;
        }

        private void Delivery_OnDisconnected(IDeliveryConnection _conn)
        { 
            var conn = (DeliveryConnection)_conn; 

            connMap.TryRemove(conn.GetHashCode(), out _);

            try
            {
                Conn_OnDisconnected?.Invoke(conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion
    }
}
