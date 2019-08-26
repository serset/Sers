using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Module.PubSub.ShareEndpoint;
using Sers.FrameWork.Util.Mail;
using System;

namespace App.SersKit.Station.Controllers.SersEvent
{

    public class SersEventSubscriber
    {
        static string[] emails = null;
        public static void Subscribe(string[] emails)
        {
            if (null != SersEventSubscriber.emails || emails == null || emails.Length == 0) return;

            SersEventSubscriber.emails = emails;

            Logger.log.Info("Subscribe SersEvent");

            string msgTitle;

            msgTitle = "SersEvent.ServiceStation.Start";
            HotPlugSubscriber.Subscribe(msgTitle, GetAction(msgTitle));

            msgTitle = "SersEvent.ServiceStation.Pause";
            HotPlugSubscriber.Subscribe(msgTitle, GetAction(msgTitle));

            msgTitle = "SersEvent.ServiceStation.Add";
            HotPlugSubscriber.Subscribe(msgTitle, GetAction(msgTitle));

            msgTitle = "SersEvent.ServiceStation.Remove";
            HotPlugSubscriber.Subscribe(msgTitle, GetAction(msgTitle));
        }

        static Action<JObject> GetAction(string msgTitle)
        {
            return (msg) => OnMessage(msgTitle, msg);
        }
        static void OnMessage(string msgTitle, JObject msg)
        {
            #region 发送邮件
            try
            {
                /*
                  "serviceStationInfo": {
    "serviceStationKey": "serviceStationKey1127856313576968192",
    "stationVersion": "1.1.5.0",
    "stationName": "StationDemo",
    "info": null
  },
                 */
                var html = "<h1>" + msgTitle + "</h1>";

                html += "<br/>stationName：" + msg.SelectToken("serviceStationInfo.stationName").ConvertToString();
                html += "<br/>时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                html += "<br/><br/><br/>serviceStationInfo：" + msg.SelectToken("serviceStationInfo").ConvertToString();
                html += "<br/><br/><br/>deviceInfo：" + msg.SelectToken("deviceInfo").ConvertToString();

                foreach (var email in emails)
                {
                    MailHelp.Send(email, msgTitle, html, true);
                }

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            #endregion

        }
    }
}
