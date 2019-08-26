using System;
using Sers.Core.Module.Log;
using Sers.Core.Module.PubSub.ShareEndpoint;
using Sers.Core.Util.SsError;

namespace App.SersKit.Station.Service.ErrorCollector
{
    /*
     使用demo：

          App.SersKit.Station.Service.ErrorCollector.ErrorCollectorService.PushError(new App.SersKit.Station.Service.ErrorCollector.ErrorItem
                {
                    when= "派单时短信提醒客户",
                    reason= "无法获取装修商手机号",
                    errorType = "ali短信发送失败",                 
                    stationName = "Order",
                    ssError = new Sers.Core.Util.SsError.SsError { errorTag = "errorTag" }.Serialize(),
                    occurTime = DateTime.Now,
                    content = "{}",
                    extData = "{}"                  
                });

         
         */
    public class ErrorCollectorService
    {
       
        public static void PushError(ErrorItem error)
        {
            try
            {
                Publisher.Publish("SersKit.Error", error);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }           
        }

    }

    #region model
    public class ErrorItem
    {

        /// <summary>
        /// 错误发生时机。如 "派单时短信提醒客户"
        /// </summary>
        public string when { get; set; }
        /// <summary>
        /// 错误原因。如 "无法获取装修商手机号"
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 错误类型。如 "ali云短信发送失败"
        /// </summary>
        public string errorType { get; set; }

        /// <summary>
        /// 发生错误站点的名称
        /// </summary>
        public string stationName { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public string ssError { get; set; }
        /// <summary>
        /// 错误发生时间
        /// </summary>
        public DateTime? occurTime { get; set; }

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
