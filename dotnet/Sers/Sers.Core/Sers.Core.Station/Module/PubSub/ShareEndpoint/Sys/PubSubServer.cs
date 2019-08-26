using Sers.Core.Extensions;
using Sers.Core.Module.Api.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.Module.Log;
using Sers.Core.Module.Message;

namespace Sers.Core.Module.PubSub.ShareEndpoint.Sys
{
    public class PubSubServer
    {
        public static readonly PubSubServer Instance = new PubSubServer();

        #region mq

        public void Conn_OnDisconnected(string connGuid)
        {
            //移除conn的所有订阅
            foreach (var msgTitle in subscriberMap.Keys.ToList())
            {
                SubscribeCancel(connGuid, msgTitle);
            }
        }

        public void OnReceiveMessage(string connGuid, ArraySegment<byte> messageData)
        {
            DealMessageFrame(connGuid,new SersFile().Unpack(messageData));
        }
        /// <summary>
        /// string connGuid,  List<ArraySegment<byte>> messageData
        /// </summary>
        public Action<string, List<ArraySegment<byte>>> OnSendMessage;

        #endregion

      
        

        void DealMessageFrame(string connGuid,SersFile frame)
        {
            try
            {
                byte msgType = frame.GetFile(0).AsSpan()[0];
                string msgTitle;
                switch (msgType)
                {
                    case (byte)EFrameType.publish:
                        msgTitle = Serialization.Serialization.Instance.Deserialize<string>(frame.GetFile(1));
                        Publish(msgTitle, frame.GetFile(2));
                        break;
                    case (byte)EFrameType.subscribe:
                        msgTitle = Serialization.Serialization.Instance.Deserialize<string>(frame.GetFile(1));
                        Subscribe(connGuid,msgTitle);
                        break;
                    case (byte)EFrameType.subscribeCancel:
                        msgTitle = Serialization.Serialization.Instance.Deserialize<string>(frame.GetFile(1));
                        SubscribeCancel(connGuid, msgTitle);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        /// <summary>
        /// 消息订阅者   msgTitle ->    connGuid List
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<string,string>> subscriberMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        void Publish(string msgTitle,ArraySegment<byte> msgData)
        {
            if (!subscriberMap.TryGetValue(msgTitle, out var connGuidList)) return;   

            //message,msgTitle,msgData
            var frame = new SersFile().SetFiles(
                new[] { (byte)EFrameType.message }.BytesToArraySegmentByte(),
                 Serialization.Serialization.Instance.Serialize(msgTitle).BytesToArraySegmentByte(),
                 msgData
                ).Package().ByteDataToBytes();
        
            foreach (var item in connGuidList)
            {               
                OnSendMessage?.Invoke(item.Key, new List<ArraySegment<byte>> { frame.BytesToArraySegmentByte() });
            }
        }


        public void Subscribe(string connGuid,string msgTitle)
        {
            lock (this)
            {
                var connGuidList = subscriberMap.GetOrAdd(msgTitle, (key) => new ConcurrentDictionary<string, string>());

                connGuidList.TryAdd(connGuid, connGuid);
            }
        }

        public void SubscribeCancel(string connGuid, string msgTitle)
        {
            lock (this)
            {
                if (!subscriberMap.TryGetValue(msgTitle, out var connGuidList)) return;
                connGuidList.TryRemove(connGuid, out _);
                if (connGuidList.IsEmpty)
                {
                    subscriberMap.TryRemove(msgTitle, out _);
                }
            }
        }
    }
}
