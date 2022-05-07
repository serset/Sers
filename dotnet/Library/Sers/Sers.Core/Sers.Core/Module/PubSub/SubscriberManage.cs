using Vit.Core.Module.Log;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.PubSub
{
    public class SubscriberManage
    {
        public static readonly SubscriberManage Instance = new SubscriberManage();


        public SubscriberManage()
        {
            MessageClient.Instance.Message_Consumer = Message_Consumer;
        }


        /// <summary>
        /// 消息订阅者   msgTitle ->    {Subscriber hashcode:Subscriber}
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<int, ISubscriber>> subscriberMap = new ConcurrentDictionary<string, ConcurrentDictionary<int, ISubscriber>>();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Message_Consumer(string msgTitle, ArraySegment<byte> msgData)
        {
            if (!subscriberMap.TryGetValue(msgTitle, out var subscriberList)) return;


            foreach (var kv in subscriberList)
            {
                ConsumeMsg(kv.Value);
            }

            //function
            void ConsumeMsg(ISubscriber subscriber)
            {
                Task.Run(() =>
                {
                    try
                    {
                        subscriber.OnGetMessage(msgData);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("EndpointManage.Message_Consume", ex);                         
                    }                    
                });
            }
        }

        public void Message_Subscribe(ISubscriber subscriber)
        {
            lock (this)
            {
                var subscriberList = subscriberMap.GetOrAdd(subscriber.msgTitle, (key) => new ConcurrentDictionary<int, ISubscriber>());

                if (subscriberList.IsEmpty)
                {
                    MessageClient.Instance.Message_Subscribe(subscriber.msgTitle);
                }
                subscriberList.TryAdd(subscriber.GetHashCode(), subscriber);
            }
        }
        public void Message_UnSubscribe(HotPlugSubscriber subscriber)
        {
            lock (this)
            {
                if (!subscriberMap.TryGetValue(subscriber.msgTitle, out var subscriberList)) return;
                subscriberList.TryRemove(subscriber.GetHashCode(), out _);

                if (subscriberList.IsEmpty)
                {
                    subscriberMap.TryRemove(subscriber.msgTitle, out _);
                    MessageClient.Instance.Message_UnSubscribe(subscriber.msgTitle);
                }
            }     
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Message_Publish(string msgTitle, ArraySegment<byte> msgData)
        {
            MessageClient.Instance.Message_Publish(msgTitle, msgData);
        }
    }
}
