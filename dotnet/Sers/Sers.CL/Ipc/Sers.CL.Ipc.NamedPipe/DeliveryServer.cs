using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.CL.Ipc.NamedPipe
{
    public class DeliveryServer: IDeliveryServer
    {

        /// <summary>
        /// 如： "Sers.CL.Ipc"
        /// </summary>
        public string pipeName = "Sers.CL.Ipc";


        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; } 


        /// <summary>
        ///  connHashCode -> DeliveryConnection
        /// </summary>
        readonly ConcurrentDictionary<int, DeliveryConnection> connMap = new ConcurrentDictionary<int, DeliveryConnection>();

        public IEnumerable<IDeliveryConnection> ConnectedList => connMap.Values.Select(conn => ((IDeliveryConnection)conn));


        LongTaskHelp tcpListenerAccept_BackThread = new LongTaskHelp();


        #region Start     


        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            try
            {
                Logger.Info("[CL.DeliveryServer] Ipc.NamedPipe, starting... pipeName:" + pipeName);

                #region (x.1)检测命名管道是否已经在使用
                try
                {

                    if (File.Exists("\\\\.\\pipe\\" + pipeName)) 
                    {
                        Logger.Info("[CL.DeliveryServer] Ipc.NamedPipe, not started.pipeName already exists!");
                        return false;
                    }

                    //String[] listOfPipes = System.IO.Directory.GetFiles(@"\\.\pipe\");
                    //foreach(var t in listOfPipes) 
                    //{
                    //    Logger.Info("pipeName:   " + t);
                    //}

                    //using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                    //{
                    //    client.Connect(100);
                    //    if (client.IsConnected)
                    //    {
                    //        Logger.Info("[CL.DeliveryServer] Ipc.NamedPipe, not started.pipeName already exists!");
                    //        return false;
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }               
                #endregion




                #region (x.2)启动Task监听listener
                tcpListenerAccept_BackThread.action = () =>
                {
                    try
                    {
                        while (true)
                        {
                            NamedPipeServerStream server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances,PipeTransmissionMode.Byte,PipeOptions.Asynchronous);

                            //Console.WriteLine($"[{id}]服务器管道已创建。");

                            // 等待客户端的连接
                            server.WaitForConnection();

                            Delivery_OnConnected(server);
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

                Logger.Info("[CL.DeliveryServer] Ipc.NamedPipe, started.");
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

            //(x.2) close
            Task.Run(() =>
            {
                Logger.Info("[CL.DeliveryServer] Ipc.NamedPipe, stop...");       

                tcpListenerAccept_BackThread.Stop();                 

                Logger.Info("[CL.DeliveryServer] Ipc.NamedPipe, stoped");

            });

        }
        #endregion


        #region Delivery_Event


        private DeliveryConnection Delivery_OnConnected(PipeStream client)
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
