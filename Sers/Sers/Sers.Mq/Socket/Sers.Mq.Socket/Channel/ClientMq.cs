using Sers.Core.Module.Log;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Sers.Core.Mq;
using System.Threading;
using Sers.Core.Util.Threading;
using Sers.Core.Module.Serialization;
using Sers.Core.Extensions;

namespace Sers.Mq.Socket.Channel
{
    public class ClientMq: IClientMq
    {
        
        public Action Conn_OnDisconnected
        {
            set
            {
                _mqConnect.OnDisconnected = (_) => {
                    value();
                };
            }
        }
        public Action<Exception> OnException
        {
            set
            {
                _mqConnect.OnException = value;
            }
        }


        public ClientMqConfig config { get; private set; }




        public bool IsConnected => _mqConnect.IsConnected;

        MqConnect _mqConnect = new MqConnect();


        public ClientMq(ClientMqConfig config)
        {
            this.config = config;
        }




        #region (x.1) Connect Close Dispose

        #region Connect


        public bool Connect()
        {
            //服务已启动
            if (_mqConnect.IsConnected) return false;

            #region 连接服务器
            Logger.Info("[消息队列 Socket Client] 连接服务器,host：" + config.host + " port:" + config.port);
            TcpClient client;
            try
            {
                client = new System.Net.Sockets.TcpClient(config.host, config.port);
            }

            catch (Exception ex)
            {
                //服务启动失败
                Logger.log.Error("[消息队列 Socket Client] 连接服务器 出错", ex);
                return false;
            }



            #endregion

            if (!_mqConnect.Init(client,config))
            {
                _mqConnect.Close();
                Logger.Info("[消息队列 Socket Client] 无法连接服务器。");
                return false;
            }

            //发送身份验证
            if (!checkSecretKey()) return false;

            ping_BackThread.action = Ping_Thread;
            ping_BackThread.Start();

            Logger.Info("[消息队列 Socket Client] 已连接服务器。");
            return true;


            #region checkSecretKey
         
            bool checkSecretKey()
            {
                //发送身份验证
                Logger.Info("[消息队列 Socket Client] 发送身份验证请求...");

                var secretKey = config.secretKey;


                var reply = SendRequest(Serialization.Instance.Serialize(secretKey).BytesToByteData());
                if (Serialization.Instance.Deserialize<string>(reply) == "true")
                {
                    Logger.Info("[消息队列 Socket Client] 身份验证通过");
                    return true;
                }
                Logger.Info("[消息队列 Socket Client] 身份验证失败");
                return false;

            }
            #endregion

        }

        #endregion


        #region Close Dispose

        ~ClientMq()
        {
            Dispose();
        }


        public void Dispose()
        {
            Close();
            _mqConnect.Dispose();
        }

        public void Close()
        {
            if (!IsConnected) return;

            Logger.Info("[消息队列 Socket Client] 准备断开连接");
            try
            {
                ping_BackThread.Stop();
            }
            catch (Exception ex)
            {
                Logger.log.Error("[消息队列 Socket Client] 准备断开连接 出错",ex);
            }

            try
            {
                _mqConnect.Close();
            }
            catch (Exception ex)
            {
                Logger.log.Error("[消息队列 Socket Client] 准备断开连接 出错", ex);
            }
            Logger.Info("[消息队列 Socket Client] 已断开连接。");
        }


        #endregion

        #endregion




        #region (x.2) Message 

        public void SendMessage(List<ArraySegment<byte>> msgData)
        {
            _mqConnect.SendMessage(msgData);
        }
        public Action<ArraySegment<byte>> OnReceiveMessage { get => _mqConnect.OnReceiveMessage; set => _mqConnect.OnReceiveMessage = value; }
        #endregion


        #region (x.3) ReqRep        


        public Func<ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest
        {
            set => _mqConnect.OnReceiveRequest = value;
            get => _mqConnect.OnReceiveRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ArraySegment<byte> SendRequest(List<ArraySegment<byte>> requestData)
        {
            var flag = _mqConnect.SendRequest(requestData, out var replyData);
            return replyData;
        }

        #endregion



        #region (x.4) Ping_Thread

        LongTaskHelp ping_BackThread = new LongTaskHelp();
        void Ping_Thread()
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        bool disconnected = true;
                        try
                        {
                            disconnected = !_mqConnect.Ping();
                        }
                        catch (Exception ex)
                        {
                            Logger.log.Error(ex);
                        }

                        if (disconnected)
                        {
                            try
                            {
                                _mqConnect.Close();
                            }
                            catch (Exception ex)
                            {
                                Logger.log.Error(ex);
                            }
                            return;
                        }
                        Thread.Sleep(config.pingInterval);
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.log.Error(ex);
                    //OnException?.Invoke(ex);
                }
            }
        }
        #endregion

    }
}
