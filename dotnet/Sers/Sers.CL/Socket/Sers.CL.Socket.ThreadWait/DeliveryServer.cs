using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Core.Util.Net;
using Vit.Core.Util.Threading;

namespace Sers.CL.Socket.ThreadWait
{
    public class DeliveryServer: IDeliveryServer
    {
        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager;

        /// <summary>
        /// 服务端 监听地址。若不指定则监听所有网卡。例如： "127.0.0.1"、"sersms.com"。
        /// </summary>
        public string host = null;
        /// <summary>
        /// 服务端 监听端口号(默认 4501)。例如： 4501。
        /// </summary>
        public int port = 4501;

        public Action<IDeliveryConnection> Conn_OnDisconnected { private get; set; }
        public Action<IDeliveryConnection> Conn_OnConnected { private get; set; }
                           



        /// <summary>
        /// listener为null 表示没有监听线程在运行
        /// </summary>
        private TcpListener listener = null;


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
                if ( listener!=null ) return false;

                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,starting... host:" + host + " port:" + port);

                // IPEndPoint类将网络标识为IP地址和端口号
                IPEndPoint localEndPoint = new IPEndPoint(String.IsNullOrEmpty(host) ? IPAddress.Any : NetHelp.ParseToIPAddress(host), port);
                listener = new TcpListener(localEndPoint);
                listener.Start();

                #region (x.2)启动Task监听listener
                tcpListenerAccept_BackThread.action = () =>
                {
                    try
                    {
                        while (true)
                        {
                            Delivery_OnConnected(listener.AcceptTcpClient());
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
            if (listener==null) return;

            //(x.1) stop conn
            ConnectedList.ToList().ForEach(Delivery_OnDisconnected);            
            connMap.Clear();

            //(x.2) close socket
            Task.Run(() =>
            {
                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,stop...");       

                tcpListenerAccept_BackThread.Stop();
                try
                {
                    //在ubuntu中 listener.Stop() 会堵塞线程过长的时间，故此处加时间限制
                    Task task = new Task(() =>
                    {
                        try
                        {
                            listener.Stop();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    });

                    task.Start();
                    //最多等待2秒
                    task.Wait(2000);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    listener = null;
                }

                Logger.Info("[CL.DeliveryServer] Socket.ThreadWait,stoped");

            });

        }
        #endregion


        #region Delivery_Event


        private DeliveryConnection Delivery_OnConnected(TcpClient client)
        {
            var conn = new DeliveryConnection();
            conn.securityManager = securityManager;
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
