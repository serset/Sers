using Sers.Core.Module.PubSub.ShareEndpoint.Sys;
using System;
using Sers.Core.Extensions;

namespace Sers.Core.Module.PubSub.ShareEndpoint
{
    public class HotPlugSubscriber:IDisposable, ISubscriber
    {
        public HotPlugSubscriber(string msgTitle)
        {
            this.msgTitle = msgTitle;
            Subscribe();
        }

        public string msgTitle { get; private set; }

        private bool subscribing = false;
        /// <summary>
        /// 开启消息订阅
        /// </summary>
        public void Subscribe()
        {
            if (subscribing) return;
            EndpointManage.Instance.Message_Subscribe(this);
            subscribing = true;
        }

        /// <summary>
        /// 关闭消息订阅
        /// </summary>
        public void SubscribeCancel()
        {
            if (subscribing)
            {
                EndpointManage.Instance.Message_SubscribeCancel(this);
                subscribing = false;
            }
            
        }

        void ISubscriber.OnMessage(ArraySegment<byte> msgBody)
        {
            OnMessage?.Invoke(msgBody);
        }


        public Action<ArraySegment<byte>> OnMessage;

        #region IDisposable Support
       
         ~HotPlugSubscriber()
        {             
            Dispose();
        }

 
        public void Dispose()
        {
            SubscribeCancel();            
        }


        #endregion



        #region static Subscribe

        public static HotPlugSubscriber Subscribe(string msgTitle, Action<ArraySegment<byte>> OnMessage)
        {
            var subscriber = new HotPlugSubscriber(msgTitle);
            subscriber.OnMessage = OnMessage;
            return subscriber;
        }
        public static HotPlugSubscriber Subscribe<T>(string msgTitle, Action<T> OnMessage)
        {
            var subscriber = new HotPlugSubscriber(msgTitle);
            subscriber.OnMessage = (ArraySegment<byte> msg)=> OnMessage(msg.DeserializeFromBytes<T>());
            return subscriber;
        }

        #endregion

    }
}
