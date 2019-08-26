using Sers.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.Module.Log;
using Sers.Core.Module.Message;
using Sers.Core.Module.Mq.Mq;

namespace Sers.Core.Module.PubSub
{
    public class MessageCenterService
    {
        public static readonly MessageCenterService Instance = new MessageCenterService();

        #region mq

        public void Conn_OnDisconnected(IMqConn mqConn)
        {
            //移除conn的所有订阅
            foreach (var msgTitle in subscriberMap.Keys.ToList())
            {
                SubscribeCancel(mqConn, msgTitle);
            }
        }

        public void OnGetMessage(IMqConn mqConn, ArraySegment<byte> messageData)
        {
            SersFile frame = new SersFile().Unpack(messageData);

            try
            {
                byte msgType = frame.GetFile(0).AsSpan()[0];
                string msgTitle = frame.GetFile(1).ArraySegmentByteToString();
                switch (msgType)
                {
                    case (byte)EFrameType.publish:
                        Publish(msgTitle, frame.GetFile(2));
                        break;
                    case (byte)EFrameType.subscribe:
                        Subscribe(mqConn, msgTitle);
                        break;
                    case (byte)EFrameType.subscribeCancel:
                        SubscribeCancel(mqConn, msgTitle);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
  

        public Action<IMqConn , List<ArraySegment<byte>>> OnSendMessage;

        #endregion

      
        

       

        /// <summary>
        /// 消息订阅者   msgTitle ->    connGuid List
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<int, IMqConn>> subscriberMap = new ConcurrentDictionary<string, ConcurrentDictionary<int, IMqConn>>();

        void Publish(string msgTitle,ArraySegment<byte> msgData)
        {
            if (!subscriberMap.TryGetValue(msgTitle, out var connGuidList)) return;   

            //message,msgTitle,msgData
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.message }.BytesToArraySegmentByte(),
                 msgTitle.SerializeToArraySegmentByte(),
                 msgData
                ).Package().ByteDataToBytes();
        
            foreach (var item in connGuidList.Values)
            {               
                OnSendMessage?.Invoke(item, new List<ArraySegment<byte>> { frame.BytesToArraySegmentByte() });
            }
        }


        public void Subscribe(IMqConn mqConn, string msgTitle)
        {
            lock (this)
            {
                var connGuidList = subscriberMap.GetOrAdd(msgTitle, (key) => new ConcurrentDictionary<int, IMqConn>());

                connGuidList.TryAdd(mqConn.GetHashCode(), mqConn);
            }
        }

        public void SubscribeCancel(IMqConn mqConn, string msgTitle)
        {
            lock (this)
            {
                if (!subscriberMap.TryGetValue(msgTitle, out var connGuidList)) return;
                connGuidList.TryRemove(mqConn.GetHashCode(), out _);
                if (connGuidList.IsEmpty)
                {
                    subscriberMap.TryRemove(msgTitle, out _);
                }
            }
        }
    }
}
