using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace App.SersKit.Station.Controllers.ErrorCollector
{
    [Table("tb_error")]
    public class ErrorItem
    {
        [JsonIgnore]
        [Key]
        public long id { get; set; }

        /// <summary>
        /// 搜集的时间
        /// </summary>
        [JsonIgnore]
        public DateTime? collectTime { get; set; }

        /// <summary>
        /// 处理状态。如"已处理"
        /// </summary>
        [JsonIgnore]
        public string state { get; set; }


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
}
