using System;
using Sers.ServiceStation;
using System.Threading;
using Sers.Core.Extensions;
using Sers.FrameWork.Util.Mail;
using Sers.Core.Util.Guid;
using Sers.Core.Util.Common;
using App.SersKit.Station.Controllers.SersEvent;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {          

            ServiceStation.Init(); 

            ServiceStation.Discovery(typeof(Program).Assembly);

            ServiceStation.Start();

            if (ServiceStation.IsRunning)
            {
                SersEventSubscriber.Subscribe(Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string[]>("SersEvent.emails"));
            }


            #region demo

            /* ticket


            var ret = App.SersKit.Station.Service.Ticket.TicketService.Push(new App.SersKit.Station.Service.Ticket.TicketItem
            {

                ticket_level = "3",
                ticket_timeoutHours = 24,
                when = "派单时短信提醒客户",
                reason = "无法获取装修商手机号",
                errorType = "ali短信发送失败",
                stationName = "Order",
                occurTime = DateTime.Now,
                description = "派单时短信提醒客户(15011112222)失败",
                howToHandle = "重新发送短信给(15011112222)，内容为“尊重的商家，您的订单已显号...”",
                ssError = new Sers.Core.Util.SsError.SsError { errorTag = "errorTag" }.Serialize(),
                content = "{}",
                extData = "{}"
            });


            //*/


            /* msg
            App.SersKit.Station.Service.ErrorCollector.ErrorCollectorService.PushError(new App.SersKit.Station.Service.ErrorCollector.ErrorItem
            {
                when = "派单时短信提醒客户",
                reason = "无法获取装修商手机号",
                errorType = "ali短信发送失败",
                stationName = "Order",
                ssError = new Sers.Core.Util.SsError.SsError { errorTag = "errorTag" }.Serialize(),
                occurTime = DateTime.Now,
                content = "{}",
                extData = "{}"
            });

            //*/

            #endregion

            ServiceStation.RunAwait();

        }
    }
}
