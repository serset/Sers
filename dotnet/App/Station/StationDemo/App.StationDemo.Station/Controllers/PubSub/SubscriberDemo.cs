using Newtonsoft.Json.Linq;
using Sers.Core.Module.PubSub.ShareEndpoint;
using System;

namespace App.StationDemo.Station.Controllers.PubSub
{
    public class SubscriberDemo
    {
        #region Demo
        public static void Subscribe()
        { 
            #region Demo1
            {
                string msgTitle = "SersEvent.ServiceStation.Add";
                Action<ArraySegment<byte>> OnMessage = (msgBody) =>
                {
                    var msg = Sers.Core.Module.Serialization.Serialization.Instance.Deserialize<string>(msgBody);
                    Console.WriteLine("[Demo1][" + msgTitle + "]");
                };

                //创建消息订阅
                var subscriber = HotPlugSubscriber.Subscribe(msgTitle, OnMessage);

                //关闭消息订阅
                //subscriber.SubscribeCancel();
            }
            #endregion

            #region Demo2
            {
                string msgTitle = "SersEvent.ServiceStation.Add";
                Action<JObject> OnMessage = (msgBody) =>
                {
                    var msg = msgBody.ToString();
                    Console.WriteLine("[Demo2][" + msgTitle + "]");
                };

                //创建消息订阅
                var subscriber = HotPlugSubscriber.Subscribe(msgTitle, OnMessage);

                //关闭消息订阅
                //subscriber.SubscribeCancel();
            }
            #endregion

        }
        #endregion

    }
}
