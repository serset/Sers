﻿using System;
using System.Runtime.CompilerServices;

using Vit.Extensions.Serialize_Extensions;

namespace Sers.Core.Module.PubSub
{
    public class HotPlugSubscriber : IDisposable, ISubscriber
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
            SubscriberManage.Instance.Message_Subscribe(this);
            subscribing = true;
        }

        /// <summary>
        /// 关闭消息订阅
        /// </summary>
        public void SubscribeCancel()
        {
            if (subscribing)
            {
                SubscriberManage.Instance.Message_UnSubscribe(this);
                subscribing = false;
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ISubscriber.OnGetMessage(ArraySegment<byte> msgBody)
        {
            OnGetMessage?.Invoke(msgBody);
        }

        /// <summary>
        /// 回调不会在消息线程中执行。 will be invoked in Task.Run(()=>{     });
        /// </summary>
        public Action<ArraySegment<byte>> OnGetMessage;

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
        /// <summary>
        /// 回调不会在消息线程中执行。 will be invoked in Task.Run(()=>{     });
        /// </summary>
        /// <param name="msgTitle"></param>
        /// <param name="OnMessage"></param>
        /// <returns></returns>
        public static HotPlugSubscriber Subscribe(string msgTitle, Action<ArraySegment<byte>> OnMessage)
        {
            var subscriber = new HotPlugSubscriber(msgTitle);
            subscriber.OnGetMessage = OnMessage;
            return subscriber;
        }

        /// <summary>
        /// 回调不会在消息线程中执行。 will be invoked in Task.Run(()=>{     });
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgTitle"></param>
        /// <param name="OnMessage"></param>
        /// <returns></returns>
        public static HotPlugSubscriber Subscribe<T>(string msgTitle, Action<T> OnMessage)
        {
            var subscriber = new HotPlugSubscriber(msgTitle);
            subscriber.OnGetMessage = (ArraySegment<byte> msg) => OnMessage(msg.DeserializeFromArraySegmentByte<T>());
            return subscriber;
        }

        #endregion

    }
}
