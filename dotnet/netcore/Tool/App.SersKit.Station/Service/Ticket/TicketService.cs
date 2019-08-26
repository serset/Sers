using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using System;

namespace App.SersKit.Station.Service.Ticket
{

    /*
      
     使用demo：
     
         var ret=App.SersKit.Station.Service.Ticket.TicketService.Push(new App.SersKit.Station.Service.Ticket.TicketItem {

                    ticket_level="3",
                    ticket_timeoutHours=24,
                    when = "派单时短信提醒客户",
                    reason= "无法获取装修商手机号",
                    errorType = "ali短信发送失败",                 
                    stationName = "Order",
                    occurTime = DateTime.Now,
                    description = "派单时短信提醒客户(15011112222)失败",
                    howToHandle = "重新发送短信给(15011112222)，内容为“尊重的商家，您的订单已显号...”",
                    ssError = new Sers.Core.Util.SsError.SsError { errorTag = "errorTag" }.Serialize(),                  
                    content = "{}",
                    extData= "{}"                   
                });        
         
         
         */
    public class TicketService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static ApiReturn Push(TicketItem ticket)
        {
            return ApiClient.CallRemoteApi<ApiReturn>("/SersKit/Ticket/push", ticket);
        }      
    }


    #region model
    public class TicketItem
    {



        #region 工单系统


        /// <summary>
        /// 工单等级
        /// </summary>
        public string ticket_level { get; set; }


        /// <summary>
        /// 工单超时时间（小时）
        /// </summary>
        public int? ticket_timeoutHours { get; set; }


        #endregion


        /// <summary>
        /// 工单发生时机。如 "派单时短信提醒客户"
        /// </summary>
        public string when { get; set; }
        /// <summary>
        /// 工单原因。如 "无法获取装修商手机号"
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 工单类型。如 "ali云短信发送失败"
        /// </summary>
        public string errorType { get; set; }

        /// <summary>
        /// 推送工单的站点
        /// </summary>
        public string stationName { get; set; }


        /// <summary>
        /// 工单发生时间
        /// </summary>
        public DateTime? occurTime { get; set; }


        /// <summary>
        /// 工单描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 工单如何处理
        /// </summary>
        public string howToHandle { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public string ssError { get; set; }

        /// <summary>
        /// json 记录错误内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string extData { get; set; }

    }
    #endregion
}
