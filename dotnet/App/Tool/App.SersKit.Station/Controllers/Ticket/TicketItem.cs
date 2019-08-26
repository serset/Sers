using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace App.SersKit.Station.Controllers.ErrorCollector
{
    [Table(TicketItem.tableName)]
    public class TicketItem
    {
        public const string tableName = "tb_ticket";

        [JsonIgnore]
        [Key]
        public long id { get; set; }

        /// <summary>
        /// 搜集的时间
        /// </summary>
        [JsonIgnore]
        public DateTime? collectTime { get; set; }

        #region 工单系统

        /// <summary>
        /// 工单id
        /// </summary>
        [JsonIgnore]
        public long? ticket_id { get; set; }

        /// <summary>
        /// 工单等级
        /// </summary>
        public string ticket_level { get; set; }


        /// <summary>
        /// 工单超时时间（小时）
        /// </summary>
        public int? ticket_timeoutHours { get; set; }

        /// <summary>
        /// 处理状态。如 "":无需处理，"open":正在受理状态        "close":工单受理完毕
        /// </summary>
        [JsonIgnore]
        public string ticket_state { get; set; }

        /// <summary>
        /// 处理日志，json格式存储处理日志。如：   [{handleTime:"2019-02-02 02:02:02",handler:'123@163.com'}]
        /// </summary>
        [JsonIgnore]
        public string ticket_log { get; set; }
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
}
