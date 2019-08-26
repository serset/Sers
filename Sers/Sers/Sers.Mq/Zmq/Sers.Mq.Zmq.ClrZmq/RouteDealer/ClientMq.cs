using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Module.Serialization;
using Sers.Core.Mq;
using Sers.Core.Util.Common;
using Sers.Core.Util.Threading;
using ZeroMQ;

namespace Sers.Mq.Zmq.ClrZmq.RouteDealer
{
    public class ClientMq : IClientMq
    {

 

        public Action Conn_OnDisconnected { set; get; }
        public Action<Exception> OnException { set; get; }



      


        public bool IsConnected => null == socketConnect ? false : (socketConnect.IsConnected && state==1);

        /// <summary>
        /// 
        /// </summary>
        string connGuid;


        SocketConnect socketConnect = new SocketConnect();

        public ClientMqConfig config { get; private set; }

        public ClientMq(ClientMqConfig config)
        {
            this.config = config;
        }


        #region Connect


        public bool Connect()
        {
            return Connect(null);
        }


        public bool Connect(string connGuid)
        {
            Logger.Info("[消息队列 Zmq Client] 连接服务器,host:" + config.host + " port:" + config.port);
            if (string.IsNullOrEmpty(connGuid))
            {
                connGuid = CommonHelp.NewGuid();
            }
            this.connGuid = connGuid;


            var socket = new ZSocket(ZSocketType.DEALER);


            socket.SetOption(ZSocketOption.IDENTITY, connGuid);
            socket.Connect("tcp://" + config.host + ":" + config.port);


            socketConnect.OnGetMsg = Zmq_OnGetMsg;
            socketConnect.Init(socket,config.workThreadCount);

            if (!Ping())
            {
                socketConnect.Close();
                Logger.Info("[消息队列 Zmq Client] 无法连接服务器。");
                return false;
            }

            ping_BackThread.action = Ping_Thread;
            ping_BackThread.Start();

            Logger.Info("[消息队列 Zmq Client] 已连接服务器。");

            return true;
        }

        #endregion


        #region Close Dispose


        public void Dispose()
        {
            Close();         
        }

        public void Close()
        {
            if (null == socketConnect) return;
            var _socketConnect = socketConnect;
            socketConnect = null;
            Task.Run(() =>
            {
                Logger.Info("[消息队列 Zmq Client] 准备断开连接");
                try
                {
                    Conn_OnDisconnected?.Invoke();
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);
                }

                ping_BackThread.Stop();

                _socketConnect.Dispose();                

                Logger.Info("[消息队列 Zmq Client] 已断开连接。");
            });
        }


        #endregion


        #region Ping       


        static readonly List<ArraySegment<byte>> ping_ConnectData = new List<ArraySegment<byte>>() { Serialization.Instance.Serialize(SocketConnect.MqVersion) };
        static readonly List<ArraySegment<byte>> Ping_HeartBeatData = new List<ArraySegment<byte>>() { Serialization.Instance.Serialize("PingHeartBeat") };

        public bool Ping()
        {
            int retry = config.pingRetryCount;
            while (!Ping_Try())
            {
                if ((--retry) <= 0)
                {
                    //disconnected
                    if (state == 1)
                        state = 2;
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        ///  0:尚未连接   1：已连接     2：已断开
        /// </summary>
        private byte state=0;
        bool Ping_Try()
        {
            ArraySegment<byte> replyData;
            if (state == 0)
            {
                if (Request_Send(ping_ConnectData, out  replyData, ERequestType.ping, config.pingTimeout))
                {
                    var mqVersion = Serialization.Instance.Deserialize<string>(replyData);
                    if (mqVersion == SocketConnect.MqVersion)
                    {
                        //connected
                        if (state == 0)
                            state = 1;
                        return true;
                    }
                }
                return false;
            }

            if (Request_Send(Ping_HeartBeatData, out  replyData, ERequestType.ping, config.pingTimeout))
            {
                if (replyData != null && replyData.Count >0 )
                {
                    return true;
                }
            }
            return false;
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
                        if (!Ping())
                        {
                            Close();
                            return;
                        }
                        Thread.Sleep(config.pingInterval);
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    OnException?.Invoke(ex);
                }
            }
        }
        #endregion


        #region ReqRep        

        public Func<ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest { set; get; }

        public ArraySegment<byte> SendRequest(List<ArraySegment<byte>> requestData)
        {
            var flag = Request_Send(requestData, out var replyData,ERequestType.app,config.requestTimeout);
            return replyData;
        }
        #endregion


        #region Message

        public void SendMessage(List<ArraySegment<byte>> msgData)
        {
            byte requestType = 0;

            Zmq_SendData(new[]
                {
                    msgData.ByteDataToBytes(),
                    new []{(byte)EMsgType.message, requestType }
                });
        }
        public Action<ArraySegment<byte>> OnReceiveMessage { get; set; }
        #endregion





        #region Request Manage       


        List<ArraySegment<byte>> OnReceiveAnyRequest(byte requestType, byte[] requestData)
        {
            switch ((ERequestType)requestType)
            {
                case ERequestType.app:
                    {
                        //app
                        return OnReceiveRequest(requestData);
                    }
                case ERequestType.ping:
                    {
                        return new List<ArraySegment<byte>> { requestData };
                    }
            }
            return null;
        }


        readonly ConcurrentDictionary<string, RequestInfo> Request_Map = new ConcurrentDictionary<string, RequestInfo>();

        bool Request_Send(List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData, ERequestType requestType,int requestTimeout)
        {
            string requestGuid = CommonHelp.NewGuid();

            var requestInfo = new RequestInfo();
            Request_Map[requestGuid] = requestInfo;
            requestInfo.mEvent = new AutoResetEvent(false);

            #region SendRequest
            Zmq_SendData(new[]
                {
                    requestGuid.StringToBytes(),
                    requestData.ByteDataToBytes(),
                    new []{(byte)EMsgType.request, (byte)requestType }
                });
            #endregion


            if (requestInfo.mEvent.WaitOne(requestTimeout))
            {
                replyData = requestInfo.replyData;
                return true;
            }

            Request_Map.TryRemove(requestGuid, out var _);
            replyData = null;
            return false;
        }


        void Request_OnGetMsg(string requestGuid, byte[] replyData)
        {
            if (Request_Map.TryRemove(requestGuid, out var requestInfo))
            {
                requestInfo.replyData = replyData;
                requestInfo.mEvent.Set();
            }
        }

        #endregion



        #region zmq 底层接口




        void Zmq_OnGetMsg(ZMessage msg)
        {
            if (null == msg || msg.Count == 0) return;

            var msgTypeFrame = msg[msg.Count - 1];

            var msgType = (EMsgType)msgTypeFrame.ReadByte();

            switch (msgType)
            {
                case EMsgType.reply:
                    {
                        if (3 != msg.Count) return;

                        //var requestType = (byte)msgTypeFrame.ReadByte();
                        string requestGuid = msg[0].Read().BytesToString();
                        var replyData = msg[1].Read();
                        Request_OnGetMsg(requestGuid, replyData);
                        return;
                    }
                case EMsgType.request:
                    {
                        if (3 != msg.Count) return;

                        var requestType = (byte)msgTypeFrame.ReadByte();

                        var requestGuid = msg[0].Read();
                        var requestData = msg[1].Read();

                        var replyData = OnReceiveAnyRequest(requestType, requestData);
                         
                        Zmq_SendData(new[] {
                            requestGuid,
                            replyData.ByteDataToBytes(),
                            new[] { (byte)EMsgType.reply , requestType }
                        });
                        return;
                    }
                case EMsgType.message:
                    {
                        if (2 != msg.Count) return;

                        //var requestType = (byte)msgTypeFrame.ReadByte();

                        var msgData = msg[0].Read();

                        OnReceiveMessage?.Invoke(msgData);
                        return;
                    }
            }
        }




        void Zmq_SendData(byte[][] data)
        {
            var fa = new ZFrame[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                fa[i] = new ZFrame(data[i]);
            }
            socketConnect.SendMsg(new ZMessage(fa));
        }

        #endregion
    }
}
