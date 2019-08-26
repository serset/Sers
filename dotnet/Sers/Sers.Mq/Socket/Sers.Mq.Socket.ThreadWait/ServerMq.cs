
using Sers.Core.Module.Log;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Module.Mq.Mq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Sers.Core.Util.Net;
using Sers.Core.Util.Threading;

namespace Sers.Mq.Socket.ThreadWait
{
    public class ServerMq: IServerMq
    {
        /// <summary>
        /// Mq 服务端 监听地址。若不指定则监听所有网卡。例如： "127.0.0.1"、"sersms.com"。(appsettings.json :: Sers.Mq.Config.ServerMqBuilder[x].host)
        /// </summary>
        public string host = null;
        /// <summary>
        /// Mq 服务端 监听端口号。例如： 10345。(appsettings.json :: Sers.Mq.Config.ServerMqBuilder[x].port)
        /// </summary>
        public int port = 10345;
        public Action<IMqConn> Conn_OnDisconnected { get; set; }
        public Action<IMqConn> Conn_OnConnected { get; set; }


        /// <summary>
        /// listener为null 表示没有监听线程在运行
        /// </summary>
        private TcpListener listener = null;


        /// <summary>
        /// 请勿处理耗时操作，需立即返回。收到数据事件          public delegate void Conn_OnGetFrame(IMqConn token, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        public Action<IMqConn, ArraySegment<byte>> Conn_OnGetFrame { set; get; }

        /// <summary>
        ///  connGuid -> MqConnect
        /// </summary>
        public readonly ConcurrentDictionary<int, MqConn> connMap = new ConcurrentDictionary<int, MqConn>();

        public IEnumerable<IMqConn> ConnectedList => connMap.Values.Select(conn => ((IMqConn)conn));


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

                Logger.Info("[ServerMq] Socket.ThreadWait,starting... host:" + host + " port:" + port);

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
                            DealTcpConnect(listener.AcceptTcpClient());
                        }
                    }
                    catch (Exception ex) when (!(ex is ThreadInterruptedException))
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

                Logger.Info("[ServerMq] Socket.ThreadWait,started.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;


            #region   DealTcpConnect
            void DealTcpConnect(TcpClient client)
            {
                var conn = new MqConn();
                conn.Init(client);
                conn.Conn_OnDisconnected = MqConn_OnDisconnected;           
                conn.OnGetFrame = Conn_OnGetFrame;
                conn.StartBackThreadToReceiveMsg();

                connMap[conn.GetHashCode()] = conn;
                Conn_OnConnected?.Invoke(conn);
            }

            #endregion

        }

        #endregion


        #region Stop

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (listener==null) return;

            //(x.1) stop mqConn
            ConnectedList.ToList().ForEach(MqConn_OnDisconnected);            
            connMap.Clear();

            //(x.2) close socket
            Task.Run(() =>
            {
                Logger.Info("[ServerMq] Socket.ThreadWait,stop...");
       

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

                Logger.Info("[ServerMq] Socket.ThreadWait,stoped");

            });

        }
        #endregion


        #region MqConn_OnDisconnected
        private void MqConn_OnDisconnected(IMqConn mqConn)
        { 
            var conn = (MqConn)mqConn; 

            connMap.TryRemove(conn.GetHashCode(), out _);

            try
            {
                Conn_OnDisconnected?.Invoke(mqConn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        #endregion
    }
}
