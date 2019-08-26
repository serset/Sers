using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Health;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Util.Common;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Pool;
using Sers.Core.Util.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sers.Core.Module.Mq.MqManager
{
    public class RequestAdaptor: IHealthInfo
    {

        #region GetHealthInfo
        public void GetHealthInfo(JObject info, string keyPrefix = null)
        {
            string health = "";
            health += Environment.NewLine + "curIsMap0:" + stationToMq_RequestMap_curIsMap0;
            health += Environment.NewLine + "StationToMq_RequestMap0_Len:" + StationToMq_RequestMap0.Count;
            health += Environment.NewLine + "StationToMq_RequestMap1_Len:" + StationToMq_RequestMap1.Count;

            health += Environment.NewLine + "timeoutTime:" + stationToMq_RequestMap_timeoutTime.ToString("HH:mm:ss.ffff");
            health += Environment.NewLine + "mqToStation_MessageFrameQueue_Len:" + mqToStation_MessageFrameQueue.Count;
            health += Environment.NewLine + "mqConn_Len:" + GetConnList()?.Count();
 

            info[keyPrefix + "Mq.RequestAdaptor"] = health;
        }

        #endregion

        #region config

        /// <summary>
        /// 后台处理消息的线程个数（单位个，默认2）(appsettings.json :: Sers.Mq.Config.workThreadCount)
        /// </summary>
        int workThreadCount = 2;

        /// <summary>
        /// 请求超时时间（单位ms，默认60000）(appsettings.json :: Sers.Mq.Config.requestTimeoutMs)
        /// </summary>
        public int requestTimeoutMs = 60000;

        /// <summary>
        /// 心跳测试超时时间（单位ms，默认2000）
        /// </summary>
        int heartBeatTimeoutMs = 2000;
        /// <summary>
        /// 心跳测试失败重试次数（单位次，默认3）
        /// </summary>
        int heartBeatRetryCount = 3;
        /// <summary>
        /// 心跳测试时间间隔（单位ms，默认1000）
        /// </summary>
        int heartBeatIntervalMs = 1000;


        #endregion

        public RequestAdaptor()
        {
            requestTimeoutMs = ConfigurationManager.Instance.GetByPath<int?>("Sers.Mq.Config.requestTimeoutMs") ?? 60000;

            workThreadCount = ConfigurationManager.Instance.GetByPath<int?>("Sers.Mq.Config.workThreadCount") ?? 2;

            heartBeatTimeoutMs = ConfigurationManager.Instance.GetByPath<int?>("Sers.Mq.Config.heartBeatTimeoutMs") ?? 2000;
            heartBeatRetryCount = ConfigurationManager.Instance.GetByPath<int?>("Sers.Mq.Config.heartBeatRetryCount") ?? 3;
            heartBeatIntervalMs = ConfigurationManager.Instance.GetByPath<int?>("Sers.Mq.Config.heartBeatIntervalMs") ?? 1000;
        }

        /// <summary>
        /// 会在内部线程中被调用 
        /// (IMqConn conn,Object sender, ArraySegment&lt;byte&gt; requestData,  Action&lt;object, List&lt;ArraySegment&lt;byte&gt;&gt;&gt; callback)
        /// </summary>
        public Action<IMqConn,object, ArraySegment<byte>, Action<object, List<ArraySegment<byte>>>> station_OnGetRequest;

        /// <summary>
        /// 会在内部线程中被调用
        /// </summary>
        public Action<IMqConn,ArraySegment<byte>> station_OnGetMessage;



        LongTaskHelp taskToDealMqMessage = new LongTaskHelp();
        public void Start()
        {
            //(x.1) taskToDealMqMessage
            taskToDealMqMessage.Stop();

            taskToDealMqMessage.threadName = "Mq-RequestAdaptor-dealer";
            taskToDealMqMessage.threadCount = workThreadCount;
            taskToDealMqMessage.action = MqToStation_TaskToDealMqMessage;
            taskToDealMqMessage.Start();

            //(x.2) heartBeat thread
            //法1
            //heartBeat_BackThread.Stop();
            //heartBeat_BackThread.threadName = "Mq-RequestAdaptor-heartBeat";
            //heartBeat_BackThread.threadCount = 1;
            //heartBeat_BackThread.action = HeartBeat_Thread;
            //heartBeat_BackThread.Start();

            //法2
            heartBeat_Timer.timerCallback = (state) => { HeartBeat_Loop(); };
            heartBeat_Timer.interval = heartBeatIntervalMs / 1000.0;
            heartBeat_Timer.Start();

        }
        public void Stop()
        {
            taskToDealMqMessage.Stop();

            //heartBeat_BackThread.Stop();

            heartBeat_Timer.Stop();
        }


        #region StationToMq


        #region StationToMq_RequestInfo       
        public class StationToMq_RequestInfo
        {
            public object sender;
            public Action<object, List<ArraySegment<byte>>> callback;

            public static StationToMq_RequestInfo Pop()
            {
                return ObjectPool<StationToMq_RequestInfo>.Shared.Pop();
            }

            /// <summary>
            /// 使用结束请手动调用
            /// </summary>
            public void Push()
            {
                sender = null;
                callback = null;
                ObjectPool<StationToMq_RequestInfo>.Shared.Push(this);
            }

        };
        #endregion

        DateTime stationToMq_RequestMap_timeoutTime = DateTime.Now;
        bool stationToMq_RequestMap_curIsMap0 = true;

        #region StationToMq_RequestMap
        //加一个缓冲区 定期 清理 无回应数据
        readonly ConcurrentDictionary<long, StationToMq_RequestInfo> StationToMq_RequestMap0 = new ConcurrentDictionary<long, StationToMq_RequestInfo>();
        readonly ConcurrentDictionary<long, StationToMq_RequestInfo> StationToMq_RequestMap1 = new ConcurrentDictionary<long, StationToMq_RequestInfo>();
        #endregion

        void StationToMq_RequestMap_Set(ref long guid, StationToMq_RequestInfo reqInfo)
        {
            if (stationToMq_RequestMap_timeoutTime < DateTime.Now)
            {
                lock (StationToMq_RequestMap0)
                {
                    if (stationToMq_RequestMap_timeoutTime < DateTime.Now)
                    {
                        if (stationToMq_RequestMap_curIsMap0)
                        {
                            StationToMq_RequestMap1.Clear();
                            stationToMq_RequestMap_curIsMap0 = false;
                        }
                        else
                        {
                            StationToMq_RequestMap0.Clear();
                            stationToMq_RequestMap_curIsMap0 = true;
                        }
                        stationToMq_RequestMap_timeoutTime = DateTime.Now.AddMilliseconds(requestTimeoutMs);
                    }
                }
            }
            if (stationToMq_RequestMap_curIsMap0)
            {
                StationToMq_RequestMap0[guid] = reqInfo;
            }
            else
            {
                guid *= -1;
                StationToMq_RequestMap1[guid] = reqInfo;
            }
        }

        bool StationToMq_RequestMap_TryRemove(long guid, out StationToMq_RequestInfo reqInfo)
        {
            return (guid > 0 ? StationToMq_RequestMap0 : StationToMq_RequestMap1).TryRemove(guid, out reqInfo);
        }
        #endregion


        #region StationToMq Send Request
        public void Station_SendMessageAsync(IMqConn conn, List<ArraySegment<byte>> message)
        {
            byte requestType = 0;
            SendMessageToMqAsync(conn, (byte)EFrameType.message, requestType, message);
        }

        public long Station_SendRequestAsync(IMqConn conn, Object sender, List<ArraySegment<byte>> requestData, Action<object, List<ArraySegment<byte>>> callback, ERequestType requestType = ERequestType.app)
        {
            long reqKey = CommonHelp.NewGuidLong();

            var requestInfo = StationToMq_RequestInfo.Pop();
            requestInfo.sender = sender;
            requestInfo.callback = callback;

            StationToMq_RequestMap_Set(ref reqKey, requestInfo);

            PackageReqRepFrame(reqKey, requestData, out var reqRepFrame);

            //SendRequest
            SendMessageToMqAsync(conn, (byte)EFrameType.request, (byte)requestType, reqRepFrame);
            return reqKey;
        }



        #region Station_SendRequest

        public bool Station_SendRequest(List<ArraySegment<byte>> requestData, out List<ArraySegment<byte>> replyData, IMqConn mqConn)
        {
            List<ArraySegment<byte>> _replyData = null;

            AutoResetEvent mEvent = pool_AutoResetEvent.Pop();
            mEvent.Reset();

            long reqKey = Station_SendRequestAsync(mqConn, null, requestData, (sender, replyData_) => {
                _replyData = replyData_;
                mEvent?.Set();
            });

            try
            {
                if (mEvent.WaitOne(requestTimeoutMs))
                {
                    replyData = _replyData;
                    return true;
                }
                else
                {
                    if (StationToMq_RequestMap_TryRemove(reqKey, out var requestInfo))
                    {
                        requestInfo.Push();
                    }
                    replyData = null;
                    return false;
                }
            }
            finally
            {
                var eToPush = mEvent;
                mEvent = null;
                pool_AutoResetEvent.Push(eToPush);
            }
        }


        ObjectPoolGenerator<AutoResetEvent> pool_AutoResetEvent = new ObjectPoolGenerator<AutoResetEvent>(() => new AutoResetEvent(false));
        #endregion

        #endregion





        #region MqToStation  

        readonly BlockingCollection<MqToStation_MessageFrame> mqToStation_MessageFrameQueue = new BlockingCollection<MqToStation_MessageFrame>();

        public void MqToStation_PushMessageFrame(IMqConn mqConn, ArraySegment<byte> messageFrame)
        {
            var msg = MqToStation_MessageFrame.Pop();
            msg.mqConn = mqConn;
            msg.messageFrame = messageFrame;

            mqToStation_MessageFrameQueue.Add(msg);
        }



        class MqToStation_MessageFrame
        {
            public IMqConn mqConn;
            public ArraySegment<byte>? messageFrame;


            public static MqToStation_MessageFrame Pop()
            {
                return ObjectPool<MqToStation_MessageFrame>.Shared.Pop();
            }

            /// <summary>
            /// 使用结束请手动调用
            /// </summary>
            public void Push()
            {
                mqConn = null;
                messageFrame = null;

                ObjectPool<MqToStation_MessageFrame>.Shared.Push(this);
            }

        }


        class MqToStation_RequestInfo
        {
            public IMqConn mqConn;

            //public byte requestType;

            public long reqKey;

   


            public static MqToStation_RequestInfo Pop()
            {
                return ObjectPool<MqToStation_RequestInfo>.Shared.Pop();
            }

            /// <summary>
            /// 使用结束请手动调用
            /// </summary>
            public void Push()
            {
                mqConn = null;
                ObjectPool<MqToStation_RequestInfo>.Shared.Push(this);
            }
        }

        #endregion


        #region MqToStation_OnGetFrameFromMq
        void MqToStation_OnGetFrameFromMq(IMqConn conn, ArraySegment<byte> data)
        {
            if (data.Count <= 2) return;

            EFrameType msgType = (EFrameType)data.Array[data.Offset];

            var msgData = data.Slice(2);
            switch (msgType)
            {
                case EFrameType.reply:
                    {
                        UnpackReqRepFrame(msgData, out long reqKey, out var replyData);

                        if (StationToMq_RequestMap_TryRemove(reqKey, out var requestInfo))
                        {
                            requestInfo.callback(requestInfo.sender, new List<ArraySegment<byte>> { replyData });
                            requestInfo.Push();
                        }
                        return;
                    }
                case EFrameType.request:
                    {
                        byte requestType = data.Array[data.Offset + 1];

                        var reqInfo = MqToStation_RequestInfo.Pop();
                        reqInfo.mqConn = conn;
                        UnpackReqRepFrame(msgData, out reqInfo.reqKey, out var requestData);

                        MqToStation_OnGetRequest(reqInfo, requestType, requestData);
                        return;
                    }
                case EFrameType.message:
                    {
                        station_OnGetMessage?.Invoke(conn, msgData);
                        return;
                    }
            }
        }
        #endregion


        #region MqToStation_OnGetRequest

        const string MqVersion = "Sers.Mq.Socket.v1";
        
        void MqToStation_OnGetRequest(MqToStation_RequestInfo reqInfo, byte requestType, ArraySegment<byte> requestData)
        {
            switch ((ERequestType)requestType)
            {
                case ERequestType.app:
                    {
                        //app
                        station_OnGetRequest(reqInfo.mqConn, reqInfo, requestData, MqToStation_WriteReplyToMq);
                        return;
                    }
                case ERequestType.heartBeat:
                    {
                        var mqVersion = requestData.ArraySegmentByteToString();
                        if (mqVersion == MqVersion)
                        {
                            // send reply to mq
                            MqToStation_WriteReplyToMq(reqInfo, new List<ArraySegment<byte>> { requestData });
                        }
                        else
                        {
                            // send reply to mq
                            MqToStation_WriteReplyToMq(reqInfo, new List<ArraySegment<byte>> { "error".SerializeToArraySegmentByte() });
                        }
                        return;
                    }
            }
        }      
        #endregion


        #region MqToStation_WriteReplyToMq
        private void MqToStation_WriteReplyToMq(object sender, List<ArraySegment<byte>> replyData)
        {
            MqToStation_RequestInfo reqInfo = sender as MqToStation_RequestInfo;
            var mqConn = reqInfo.mqConn;
            var reqKey = reqInfo.reqKey;
            reqInfo.Push();

            PackageReqRepFrame(reqKey, replyData, out var repFrame);

            SendMessageToMqAsync(mqConn, (byte)EFrameType.reply,0, repFrame);
        }

        #endregion


        #region MqToStation_TaskToDealMqMessage
        private void MqToStation_TaskToDealMqMessage()
        {
       
            while (true)
            {
                try
                {
                    #region TaskToDealMqMessage                        
                    while (true)
                    {
                        var msgFromQueue = mqToStation_MessageFrameQueue.Take();
                        try
                        {
                            if (msgFromQueue.messageFrame != null)
                                MqToStation_OnGetFrameFromMq(msgFromQueue.mqConn, msgFromQueue.messageFrame.Value);
                        }
                        finally
                        {
                            msgFromQueue.Push();
                        }
                    }
                    #endregion
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion




        #region SendMessageToMqAsync        
        void SendMessageToMqAsync(IMqConn conn, byte msgType, byte requestType, List<ArraySegment<byte>> data)
        {
            var item = DataPool.BytesGet(2);
            item[0] = msgType;
            item[1] = requestType;
            data.Insert(0, new ArraySegment<byte>(item, 0, 2));
            conn.SendFrameAsync(data);
        }
        #endregion



        #region HeartBeat

        readonly SersTimer heartBeat_Timer = new SersTimer();


        //readonly LongTaskHelp heartBeat_BackThread = new LongTaskHelp();

        void HeartBeat_Thread()
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        HeartBeat_Loop();

                        Thread.Sleep(heartBeatIntervalMs);
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }



        void HeartBeat_Loop()
        {
            try
            {
                var temp = HeartBeat_info_cur;
                HeartBeat_info_cur = HeartBeat_info_Before;
                HeartBeat_info_Before = temp;
                var conns = GetConnList();
                if (conns != null)
                    foreach (var conn in conns)
                    {
                        try
                        {
                            HeartBeat_CheckIfDisconnectedAndSendHeartBeat(conn);
                        }
                        catch (Exception ex) when (!(ex is ThreadInterruptedException))
                        {
                            Logger.Error(ex);
                        }
                    }
                HeartBeat_info_Before.Clear();
            }
            catch (Exception ex) when (!(ex is ThreadInterruptedException))
            {
                Logger.Error(ex);
            }
        }


        static readonly byte[] MqVersion_ba = MqVersion.SerializeToBytes();
        static List<ArraySegment<byte>> HeartBeat_Data => MqVersion_ba.BytesToByteData();

        class HeartBeatInfo
        {
            public readonly List<HeartBeatPackage> list = new List<HeartBeatPackage>();

            public bool IsDisconnected(int heartBeatRetryCount)
            {
                while (list.Count > 0 && list[0].timeouted == false)
                {
                    list.RemoveAt(0);
                }

                int timeoutCount = 0;
                int curIndex = 0;
                while (curIndex < list.Count)
                {
                    var timeouted = list[curIndex].timeouted;
                    if (timeouted == false)
                    {
                        list.RemoveRange(0, curIndex + 1);
                        curIndex = 0;
                        timeoutCount = 0;
                    }
                    else if (timeouted == true)
                    {
                        timeoutCount++;
                        curIndex++;
                        if (timeoutCount >= heartBeatRetryCount) return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
       
        }

        public Func<IEnumerable<IMqConn>> GetConnList;

        class HeartBeatPackage
        {
            public DateTime timeoutTime;
            public DateTime? replyTime;
            public IMqConn mqConn;

            /// <summary>
            /// 是否超时  true:已经超时  false:未超时   null:未知
            /// </summary>
            public bool? timeouted
            {
                get
                {
                    if (replyTime != null)
                    {
                        return timeoutTime <= replyTime.Value ;
                    }

                    if (timeoutTime <= DateTime.Now) return true;
                    return null;
                }
            }
        }
        Dictionary<IMqConn, HeartBeatInfo> HeartBeat_info_Before = new Dictionary<IMqConn, HeartBeatInfo>();
        Dictionary<IMqConn, HeartBeatInfo> HeartBeat_info_cur = new Dictionary<IMqConn, HeartBeatInfo>();
        void HeartBeat_CheckIfDisconnectedAndSendHeartBeat(IMqConn mqConn)
        {
            if (HeartBeat_info_Before.TryGetValue(mqConn, out var info))
            {
                if (info.IsDisconnected(heartBeatRetryCount))
                {
                    Logger.Info("[MqManager.RequestAdaptor]HeartBeat,conn disconnected. connTag:" + mqConn.connTag);
                    mqConn.Close();
                    return;
                }
            }
            else
            {
                info = new HeartBeatInfo();
            } 

            info.list.Add(HeartBeat_Send(mqConn));

            HeartBeat_info_cur[mqConn] = info;             

        }

        HeartBeatPackage HeartBeat_Send(IMqConn mqConn)
        {
            var p = new HeartBeatPackage() { timeoutTime = DateTime.Now.AddMilliseconds(heartBeatTimeoutMs), mqConn = mqConn };
            Station_SendRequestAsync(mqConn, p, HeartBeat_Data, HeartBeat_callback, ERequestType.heartBeat);
            return p;
        }

        void HeartBeat_callback(object sender,List<ArraySegment<byte>> replyData)
        {
            //if (replyData.Count <= 0) return;

            HeartBeatPackage package = sender as HeartBeatPackage;

            if (MqVersion != replyData?.ByteDataToString())
            {
                Logger.Info("[MqManager.RequestAdaptor]HeartBeat_callback,MqVersion not match,will stop conn. connTag:" + package.mqConn.connTag+ "  replyData:"+ replyData.ByteDataToString());
                Task.Run((Action)package.mqConn.Close);
                return;
            }           
            package.replyTime = DateTime.Now;
        }


        #endregion




        #region static ReqRepFrame                  
        /*
                    //ReqRepFrame 消息帧(byte[])	 
                    第1部分： 请求标识（reqKey）(long)			长度为8字节
                    第2部分： 消息内容(oriMsg)
        */
        internal static void UnpackReqRepFrame(ArraySegment<byte> reqRepFrame, out long reqKey, out ArraySegment<byte> oriMsg)
        {
            //第1帧            
            reqKey = reqRepFrame.Slice(0, 8).ArraySegmentByteToInt64();

            //第2帧
            oriMsg = reqRepFrame.Slice(8);
        }

        /// <summary>
        /// 注：调用后，oriMsg会错乱
        /// </summary>
        /// <param name="reqKey"></param>
        /// <param name="oriMsg"></param>
        /// <param name="reqRepFrame"></param>
        static void PackageReqRepFrame(long reqKey, List<ArraySegment<byte>> oriMsg, out List<ArraySegment<byte>> reqRepFrame)
        {
            /*
            reqRepFrame = DataPool.ByteDataGet();

            //第1帧 
            reqRepFrame.Add(reqKey.Int64ToBytes().BytesToArraySegmentByte());

            //第2帧
            if (null != oriMsg) reqRepFrame.AddRange(oriMsg);

            /*/
            reqRepFrame = oriMsg ?? DataPool.ByteDataGet();

            //reqKey
            reqRepFrame.Insert(0, reqKey.Int64ToBytes().BytesToArraySegmentByte());
            //*/
        }

     
        #endregion



    }
}
