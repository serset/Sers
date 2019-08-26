using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    public class ServerMq : IServerMq
    {

 

        public Action<string> Conn_OnConnected { get; set; }
        public Action<string> Conn_OnDisconnected { get; set; }
        public Action<Exception> OnException { get; set; }


        public bool IsRunning  => socketConnect.IsConnected;


        public bool ConnIsOnline(string connGuid)
        {
            return connMap.ContainsKey(connGuid);
        }


        public List<string> GetConnGuidList()
        {
            var list = from n in connMap select n.Key;
            return list.ToList();
        }



        readonly SocketConnect socketConnect = new SocketConnect();

        class ClientInfo
        {
            public ClientInfo()
            {
                connectedTime = DateTime.Now;
            }
            public string connGuid;
            public DateTime connectedTime;

        }

        /// <summary>
        /// connGuid -> ClientInfo
        /// </summary>
        ConcurrentDictionary<string, ClientInfo> connMap = new ConcurrentDictionary<string, ClientInfo>();

        /// <summary>
        /// 
        /// </summary>
        string connGuid = CommonHelp.NewGuid();


        public ServerMqConfig config { get; private set; }

        public ServerMq(ServerMqConfig config)
        {
            this.config = config;
        }




        #region Start

        public bool Start()
        {

            Logger.Info("[消息队列 Zmq Server] 准备打开, port:" + config.port);
            var dealer = new ZSocket(ZSocketType.ROUTER);


            dealer.SetOption(ZSocketOption.IDENTITY, connGuid);
            dealer.Bind("tcp://*:" + config.port);


            socketConnect.Init(dealer, config.workThreadCount);
            socketConnect.OnGetMsg = Zmq_OnGetMsg;

            ping_BackThread.action = Ping_Thread;
            ping_BackThread.Start();

            Logger.Info("[消息队列 Zmq Server] 已打开。");
            return true;
        }
        #endregion



        #region Close Dispose

        public void Dispose()
        {
            Close();
            socketConnect.Dispose();
        }


        public void Close()
        {
            if (!IsRunning) return;

            Logger.Info("[消息队列 Zmq Server] 准备关闭");
            ping_BackThread.Stop();

            socketConnect.Close();

            Logger.Info("[消息队列 Zmq Server] 已关闭。");

        }
        #endregion


        void Conn_Close(string connGuid)
        {
            Task.Run(() =>
            {
                if (connMap.TryRemove(connGuid, out var clientInfo))
                {
                    Conn_OnDisconnected?.Invoke(connGuid);
                }
            });
        }




        #region Ping       

        static readonly List<ArraySegment<byte>> Ping_HeartBeatData = new List<ArraySegment<byte>>() { Serialization.Instance.Serialize("PingHeartBeat") };
 
 
        public bool Ping(string connGuid)
        {
            int retry = config.pingRetryCount;
            while (!Ping_Try(connGuid))
            {
                if ((--retry) <= 0)
                {
                    return false;
                }
            }
            return true;
        }
        bool Ping_Try(string connGuid)
        {
            if (Request_Send(connGuid,Ping_HeartBeatData, out var replyData, ERequestType.ping, config.pingTimeout))
            {
                if (replyData != null && replyData.Count > 0)
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
                        foreach (var connGuid in GetConnGuidList())
                        {
                            int retry = config.pingRetryCount;
                            while (!Ping(connGuid))
                            {
                                if ((--retry) <= 0)
                                {
                                    Conn_Close(connGuid);                                    
                                    break;
                                }
                            }
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


        /// <summary>
        /// 接受到请求（处理请求）的回调
        ///  (string connGuid, ArraySegment<byte> requestData,List<ArraySegment<byte>> replyData)
        /// </summary>
        public Func<string, ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest { get; set; }

        public bool SendRequest(string connGuid, List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData)
        {
            return Request_Send(connGuid, requestData, out replyData,ERequestType.app,config.requestTimeout);
        }
        #endregion


        #region Message

        public void SendMessage(string connGuid, List<ArraySegment<byte>> msgData)
        {
            if (!ConnIsOnline(connGuid)) return;

            byte msgTypeExt = 0;
            Zmq_SendData(connGuid, new[]
               {
                    msgData.ByteDataToBytes(),
                    new []{  (byte)EMsgType.message, msgTypeExt }
                });
        }

        /// <summary>
        ///  void OnReceiveMessage( string connGuid,ArraySegment<byte> msgData)
        /// </summary>
        public Action<string, ArraySegment<byte>> OnReceiveMessage { get; set; }
        #endregion




        #region Request Manage

        readonly ConcurrentDictionary<string, RequestInfo> Request_Map = new ConcurrentDictionary<string, RequestInfo>();

        List<ArraySegment<byte>> OnReceiveAnyRequest(byte requestType, string connGuid, byte[] requestData)
        {
            switch ((ERequestType)requestType)
            {
                case ERequestType.app:
                    {
                        //app
                        return OnReceiveRequest?.Invoke(connGuid, requestData);
                    }
                case ERequestType.ping:
                    {
                        //ping
                        if (connMap.ContainsKey(connGuid))
                        {
                            return new List<ArraySegment<byte>> { requestData };
                        }

                        var mqVersion = Serialization.Instance.Deserialize<string>(requestData);
                        if (mqVersion != SocketConnect.MqVersion)
                        {
                            return null;
                        }

                        #region event Conn_OnConnected         
                        Task.Run(() =>
                        {
                            try
                            {
                                if (connMap.TryAdd(connGuid, new ClientInfo { connGuid = connGuid }))
                                {
                                    Conn_OnConnected?.Invoke(connGuid);
                                }
                            }
                            catch (Exception ex)
                            {
                                OnException?.Invoke(ex);
                            }
                        });
                        #endregion
                        return new List<ArraySegment<byte>> { requestData };
                    }
            }
            return null;
        }


        bool Request_Send(string connGuid, List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData, ERequestType requestType, int millisecondsTimeout)
        {
            string requestGuid = CommonHelp.NewGuid();

            var requestInfo = new RequestInfo();
            requestInfo.mEvent = new AutoResetEvent(false);
            Request_Map[requestGuid] = requestInfo;


            #region SendRequest         
            Zmq_SendData(connGuid, new[]
                {
                    requestGuid.StringToBytes(),
                    requestData.ByteDataToBytes(),
                    new []{  (byte)EMsgType.request,(byte) requestType }
                });
            #endregion


            if (requestInfo.mEvent.WaitOne(millisecondsTimeout))
            {
                replyData = requestInfo.replyData;
                return true;
            }

            Request_Map.TryRemove(requestGuid, out var _);
            replyData = null;
            return false;
        }


        void Request_OnGetMsg(string connGuid, string requestGuid, byte[] replyData)
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
                        if (4 != msg.Count) return;

                        string connGuid = msg[0].ReadString();
                        string requestGuid = msg[1].Read().BytesToString();
                        var replyData = msg[2].Read();

                        Request_OnGetMsg(connGuid, requestGuid, replyData);
                        return;
                    }
                case EMsgType.request:
                    {
                        if (4 != msg.Count) return;

                        var requestType = (byte)msgTypeFrame.ReadByte();

                        string connGuid = msg[0].ReadString();
                        string requestGuid = msg[1].Read().BytesToString();
                        var requestData = msg[2].Read();

                        var replyData = OnReceiveAnyRequest(requestType, connGuid, requestData);
                        //构建Reply
                        Zmq_SendData(connGuid, new[] { requestGuid.StringToBytes(), replyData.ByteDataToBytes(), new[] { (byte)EMsgType.reply, requestType } });
                        return;
                    }
                case EMsgType.message:
                    {
                        if (3 != msg.Count) return;

                        //var requestType = (byte)msgTypeFrame.ReadByte();

                        string connGuid = msg[0].ReadString();
                        var messageData = msg[1].Read();

                        OnReceiveMessage?.Invoke(connGuid, messageData);
                        return;
                    }
            }
        }


        void Zmq_SendData(string connGuid, byte[][] data)
        {
            var fa = new ZFrame[data.Length + 1];
            fa[0] = new ZFrame(connGuid);
            for (int i = 0; i < data.Length; i++)
            {
                fa[i + 1] = new ZFrame(data[i]);
            }
            socketConnect.SendMsg(new ZMessage(fa));
        }
        #endregion
    }
}
