using Newtonsoft.Json;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace Sers.Core.Module.Sql.List
{
    public class DataFilter
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [SsExample("name")]
        public string fieldName;


        /// <summary>
        /// 操作符 { "=", "!=", "like", ">", "<" , "<=", ">=", "in", "not in"}
        /// </summary>
        [SsExample("like")]
        public string opt;


        /// <summary>
        /// 参数
        /// </summary>
        [SsExample("45")]
        public object value;


        /// <summary>
        /// 传递给数据库的参数的名称，若不指定，则使用自定参数名称
        /// </summary>
        /// <summary>
        /// 参数
        /// </summary>
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        [SsExample("order_no")]
        public string sqlParamName;


    }
}
