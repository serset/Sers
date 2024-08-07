﻿using System;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.PubSub;

using Vit.Core.Module.Log;
using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Did.SersLoader.Demo.Controllers.PubSub
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

                    var serviceStation = msgBody.ArraySegmentByteToString().Deserialize<JObject>();
                    var serviceStationName = serviceStation?["serviceStationInfo"]?["serviceStationName"]?.ConvertToString();
                    var msgContext = "[Subscribe1][" + msgTitle + "] " + serviceStationName;
                    Logger.Info(msgContext);
                    //Console.WriteLine(msgContext);
                };

                //创建消息订阅
                var subscriber = HotPlugSubscriber.Subscribe(msgTitle, OnMessage);

                //关闭消息订阅
                //subscriber.SubscribeCancel();
            }
            #endregion

            #region Demo2
            {
                string msgTitle = "SersEvent.ServiceStation.Remove";
                Action<JObject> OnMessage = (msgBody) =>
                {
                    var serviceStation = msgBody;
                    var serviceStationName = serviceStation?["serviceStationInfo"]?["serviceStationName"]?.ConvertToString();

                    var msgContext = "[Subscribe2][" + msgTitle + "] " + serviceStationName;
                    Logger.Info(msgContext);
                    //Console.WriteLine(msgContext);
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
