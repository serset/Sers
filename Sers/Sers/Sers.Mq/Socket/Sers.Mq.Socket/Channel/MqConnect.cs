using Sers.Mq.Socket.Channel.Socket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Sers.Core.Util.Common;
using System.Net.Sockets;
using Sers.Core.Module.Serialization;
using Sers.Core.Util.Pool;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;

namespace Sers.Mq.Socket.Channel
{
    public class MqConnect:IDisposable
    {
        public MqConnectConfig config { get; private set; }

        public Action<Exception> OnException { set => socketConnect.OnException = value; get => socketConnect.OnException; }

        public Action<SocketConnect> OnConnected { set; get; }

        public Action<SocketConnect> OnDisconnected { set => socketConnect.OnDisconnected = value; get => socketConnect.OnDisconnected; }



        SocketConnect socketConnect = new SocketConnect();

        public bool IsConnected => socketConnect.IsConnected;


        #region 构造函数
        public MqConnect()
        {
            socketConnect.OnGetMsg = Socket_OnGetMsg;
        }
        #endregion




        #region (x.1) Init Close Dispose

        #region Init

        /// <summary>
        /// 初始化并开启接收发送等线程
        /// </summary>
        /// <param name="client"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Init(TcpClient client,MqConnectConfig config)
        {
            this.config = config;

            socketConnect.Init(client, config.workThreadCount);

            if (!Ping())
            {
                socketConnect.Close();          
                return false;
            }

            try
            {
                OnConnected?.Invoke(socketConnect);
            }
            catch (Exception e)
            {
                Logger.log.Error("[消息队列 Socket Server] OnConnected调用失败。", e);
            }
            return true;
           
        }
        #endregion


        #region  Close Dispose



        ~MqConnect()
        {
            Dispose();
        }


        public void Dispose()
        {
            Close();
            socketConnect.Dispose();
        }
        public void Close()
        {

            try
            {
                if (IsConnected)
                {
                    socketConnect.Close();
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        #endregion

        #endregion

 
        #region (x.2) socket 底层接口

        void Socket_OnGetMsg(ArraySegment<byte> data)
        {
            var span = data.AsSpan();
             
            EMsgType msgType = (EMsgType)span[0];
            byte requestType = span[1];

            var msgData = data.Slice(2);
            switch (msgType)
            {
                case EMsgType.reply:
                    {
                        var repFrame = msgData;
                        UnpackReqRepFrame(repFrame, out long reqKey, out var replyData);

                        RequestManage_OnGetMsg(reqKey, replyData);
                        return;
                    }
                case EMsgType.request:
                    {
                        var reqFrame = msgData;
                        UnpackReqRepFrame(reqFrame, out var reqKey, out var requestData);

                        var replyData = RequestManage_OnReceiveRequest(requestType, requestData);

                        PackageReqRepFrame(reqKey, replyData, out var repFrame);

                        Socket_SendData((byte)EMsgType.reply, requestType, repFrame);
                        return;
                    }
                case EMsgType.message:
                    {
                        OnReceiveMessage?.Invoke(msgData);
                        return;
                    }
            }
        }


        void Socket_SendData(byte msgType, byte requestType, List<ArraySegment<byte>> data)
        {
            var item = DataPool.ArraySegmentByteGet(2);
            var span = item.AsSpan();
            span[0] = msgType;
            span[1] = requestType;
            data.Insert(0, item);
            socketConnect.SendMsg(data);
        }
        #endregion


   

   

        #region (x.3) Message
        public void SendMessage(List<ArraySegment<byte>> msgData)
        {
            byte requestType = 0;
            Socket_SendData((byte)EMsgType.message, requestType,msgData);
        }
        public Action<ArraySegment<byte>> OnReceiveMessage { get; set; }
        #endregion





        #region (x.4) RequestManage

        List<ArraySegment<byte>> RequestManage_OnReceiveRequest(byte requestType, ArraySegment<byte> requestData)
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
                        var mqVersion = Serialization.Instance.Deserialize<string>(requestData);
                        if (mqVersion != MqVersion)
                        {
                            return null;
                        }
                        var byteData = DataPool.ByteDataGet();
                        byteData.Add(requestData);
                        return byteData;
                    }
            }
            return null;
        }


        class RequestInfo
        {
            public ArraySegment<byte> replyData;
            public AutoResetEvent mEvent = new AutoResetEvent(false);
        };
        readonly ConcurrentDictionary<long, RequestInfo> RequestManage_RequestMap = new ConcurrentDictionary<long, RequestInfo>();


        bool RequestManage_SendRequest(List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData, ERequestType requestType, int millisecondsTimeout)
        {
            bool success = false;
            long reqKey = CommonHelp.NewGuidLong();

            var requestInfo = ObjectPool<RequestInfo>.Shared.Pop();
            //var requestInfo = new RequestInfo();      
            
            try
            {

                RequestManage_RequestMap[reqKey] = requestInfo;

                PackageReqRepFrame(reqKey, requestData, out var reqRepFrame);

                requestInfo.mEvent.Reset();

                //SendRequest           
                Socket_SendData((byte)EMsgType.request, (byte)requestType, reqRepFrame);

                success = requestInfo.mEvent.WaitOne(millisecondsTimeout);

                replyData = (success ? requestInfo.replyData : ArraySegmentByteExtensions.Null );
            }
            finally
            {
                if (!success) RequestManage_RequestMap.TryRemove(reqKey, out var _);
                ObjectPool<RequestInfo>.Shared.Push(requestInfo);
            }
            return success;

        }


        void RequestManage_OnGetMsg(long reqKey, ArraySegment<byte> replyData)
        {
            if (RequestManage_RequestMap.TryRemove(reqKey, out var requestInfo))
            {
                requestInfo.replyData = replyData;
                requestInfo.mEvent.Set();
            }
        }


        #region ReqRepFrame                  
        /*
                    //ReqRepFrame 消息帧(byte[])	 
                    第1部分： 请求标识（reqKey）(long)			长度为8字节
                    第2部分： 消息内容(oriMsg)
        */
        static void UnpackReqRepFrame(ArraySegment<byte> reqRepFrame, out long reqKey, out ArraySegment<byte> oriMsg)
        {
            //第1帧            
            reqKey = reqRepFrame.Slice(0, 8).ArraySegmentByteToInt64();

            //第2帧
            oriMsg = reqRepFrame.Slice(8);
        }

        static void PackageReqRepFrame(long reqKey, List<ArraySegment<byte>> oriMsg, out List<ArraySegment<byte>> reqRepFrame)
        {

            reqRepFrame = DataPool.ByteDataGet();

            //第1帧 
            reqRepFrame.Add(reqKey.Int64ToBytes().BytesToArraySegmentByte());

            //第2帧
            if (null != oriMsg) reqRepFrame.AddRange(oriMsg);
        }

        #endregion


        #endregion




        #region (x.5) ReqRep

        public Func<ArraySegment<byte>, List<ArraySegment<byte>>> OnReceiveRequest { set; get; }

        public bool SendRequest(List<ArraySegment<byte>> requestData, out ArraySegment<byte> replyData)
        {
            var flag = RequestManage_SendRequest(requestData, out replyData, ERequestType.app, config.requestTimeout);
            return flag;
        }
        #endregion



        #region (x.6) Ping

        public bool Ping()
        {
            int retry = config.pingRetryCount;
            while (!Ping_Try())
            {
                if ((--retry) <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        const string MqVersion = "Sers.Mq.Socket.v1";
        static readonly byte[] MqVersion_ba = Serialization.Instance.Serialize(MqVersion);
        static List<ArraySegment<byte>> ping_ConnectData => MqVersion_ba.BytesToByteData();

        bool Ping_Try()
        {
            
            if (!socketConnect.TestIsConnectedByPoll())
            {
                return false;
            }
            if (RequestManage_SendRequest(ping_ConnectData, out var replyData, ERequestType.ping, config.pingTimeout))
            {
                var mqVersion = Serialization.Instance.Deserialize<string>(replyData);
                if (mqVersion == MqVersion)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion



    }
}
