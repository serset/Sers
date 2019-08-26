
using Sers.Core.Module.Log;
using Sers.Core.Util.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Sers.Core.Mq;
using Sers.Core.Util.Threading;
using System.Threading;
using Sers.Core.Module.Serialization;
using Sers.Core.Extensions;

namespace Sers.Mq.Socket.Channel
{
    public class ServerMq: IServerMq
    {

        public Action<string> Conn_OnConnected { get; set; }
        public Action<string> Conn_OnDisconnected { get; set; }

      


        /// <summary>
        /// 
        /// </summary>
        /// <returns>是否正在监听</returns>
        public bool IsRunning => (null != listener);


    
        public bool ConnIsOnline(string connGuid)
        {
            return connMap.ContainsKey(connGuid);
        }


        public List<string> GetConnGuidList()
        {
            var list = from n in connMap select n.Key;
            return list.ToList();
        }

       


        /// <summary>
        /// listener为null 表示没有监听线程在运行
        /// </summary>
        private TcpListener listener = null;       



        /// <summary>
        ///  connGuid -> MqConnect
        /// </summary>
        ConcurrentDictionary<string, MqConnect> connMap = new ConcurrentDictionary<string, MqConnect>();

        public ServerMqConfig config { get; private set; }

        public ServerMq(ServerMqConfig config)
        {
            this.config = config;
        }


        #region Start

        LongTaskHelp tcpListenerAccept_BackThread = new LongTaskHelp();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>是否成功启动了服务。 true:成功启动服务 false：服务已启动 或 服务启动失败 </returns>
        public bool Start()
        {
            Logger.Info("[消息队列 Socket Server] 准备打开, port:" + config.port);

            #region (x.1)创建 并启动listener
            lock (this)
            {
                //服务已启动
                if (IsRunning) return false;

                try
                {
                    // IPEndPoint类将网络标识为IP地址和端口号
                    listener = new TcpListener(new IPEndPoint(IPAddress.Any, config.port));
                    listener.Start();
                }
                catch
                {
                    if (null != listener)
                    {
                        try
                        {
                            listener.Stop();
                        }
                        finally
                        {
                            listener = null;
                        }
                    }
                    throw;
                }
            }
            #endregion


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
                      Close();
                  }
              };
            tcpListenerAccept_BackThread.Start();
            #endregion

            ping_BackThread.action = Ping_Thread;
            ping_BackThread.Start();


            Logger.Info("[消息队列 Socket Server]已打开。");
            return true;

            #region function DealTcpConnect
            void DealTcpConnect(TcpClient client)
            {
                Task.Run(() =>
                {
                    string connGuid = CommonHelp.NewGuid();
                    MqConnect mqConnect = new MqConnect();
                    

                    //mqConnect.OnConnected = (tcpConn) =>
                    //{
                        mqConnect.OnReceiveRequest = (req) =>
                        {
                            // 身份验证
                            string secretKey = Serialization.Instance.Deserialize<string>(req);
                            bool success = (secretKey == config.secretKey);

                            if (success)
                            {
                                //验证通过
                                SaveToConnPool(connGuid, mqConnect);
                                return Serialization.Instance.Serialize("true").BytesToByteData();
                            }

                            //验证不通过
                            Logger.Info("[消息队列 Socket Server] 连接请求的身份验证不通过！");
                            Task.Run(()=> {
                                Thread.Sleep(100);
                                mqConnect.Close();
                            });
                          
                            return Serialization.Instance.Serialize("false").BytesToByteData();
                        };

 
                    //};

                    
                    mqConnect.Init(client,config);

                   
                });
            }
            #endregion

            #region SaveToConnPool 验证通过，保存至 认证连接池
            void SaveToConnPool(string connGuid, MqConnect mqConnect)
            {
                //mqConnect.OnConnected = (tcpConn) =>
                //{
                    mqConnect.OnReceiveRequest = (req) =>
                    {
                        return OnReceiveRequest(connGuid, req);
                    };


                    mqConnect.OnReceiveMessage = (msgData) =>
                    {
                        OnReceiveMessage(connGuid, msgData);
                    };

                    connMap[connGuid] = mqConnect;
                    try
                    {
                        Conn_OnConnected?.Invoke(connGuid);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);                 
                    }
                //};

                mqConnect.OnDisconnected = (_) =>
                {
                    try
                    {
                        connMap.TryRemove(connGuid, out var _conn);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                    try
                    {
                        Conn_OnDisconnected?.Invoke(connGuid);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                };
            }
            #endregion


        }

        #endregion


        #region Close Dispose


        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (!IsRunning) return;

            Task.Run(() =>
            {
                Logger.Info("[消息队列 Socket Server]准备关闭");
                ping_BackThread.Stop();

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


                Logger.Info("[消息队列 Socket Server]已关闭。");

            });
        }

        #endregion

        #region Ping_Thread


        LongTaskHelp ping_BackThread = new LongTaskHelp();
        void Ping_Thread()
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        foreach (var connGuid in GetConnGuidList())
                        {
                            if (!connMap.TryGetValue(connGuid, out var conn))
                                continue;

                            bool disconnected = true;
                            try
                            {
                                disconnected = !conn.Ping();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }

                            if (disconnected)
                            {
                                #region disconnected
                                try
                                {
                                    connMap.TryRemove(connGuid, out var _conn);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                try
                                {
                                    conn.Close();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                #endregion
                            }
                        }
                        Thread.Sleep(config.pingInterval);
                    }
                }
                catch (Exception ex)when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }

        #endregion



        #region ReqRep    

        /// <summary>
        /// 接受到请求（处理请求）的回调
        ///  (string connGuid, ArraySegment<byte> requestData,List<ArraySegment<byte>> replyData)
        /// </summary>
        public Func<string, ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest { get; set; }


        public bool SendRequest(string connGuid, List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData)
        {
            if (!connMap.TryGetValue(connGuid, out var conn))
            {
                replyData = ArraySegmentByteExtensions.Null;
                return false;
            }
            return conn.SendRequest(requestData, out replyData);     
        }    
        
      
        #endregion

        #region Message

        public void SendMessage(string connGuid, List<ArraySegment<byte>> msgData)
        {
            if (!connMap.TryGetValue(connGuid, out var conn))
            {
                return;
            }
            conn.SendMessage(msgData);         
        }

        /// <summary>
        ///  void OnReceiveMessage( string connGuid,ArraySegment<byte> msgData)
        /// </summary>
        public Action<string, ArraySegment<byte>> OnReceiveMessage { get; set; }
        #endregion
        


    }
}
