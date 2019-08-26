using Newtonsoft.Json;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace Sers.Core.Module.Sql.List
{
    public class DataFilter
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [SsExample("order_no")]
        [SsDescription("字段名")]
        public string fieldName;


        /// <summary>
        /// 操作符。可为 "=", "!=", "like", "&gt;", "&lt;" , "&gt;=", "&lt;=", "in", "not in"
        /// </summary>
        [SsExample("=")]
        [SsDescription("操作符。可为 \"=\", \"!=\", \"like\", \">\", \"<\" , \">=\", \"<=\", \"in\", \"not in\"")]
        public string opt;


        /// <summary>
        /// 参数
        /// </summary>
        [SsExample("45")]
        [SsDescription("参数")]
        public object value;


        /// <summary>
        /// 传递给数据库的参数的名称，若不指定，则使用自定义参数名称("sf_"+fieldName)
        /// </summary>
        /// <summary>
        /// 参数
        /// </summary>
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        [SsDescription("传递给数据库的参数的名称，若不指定，则使用自定义参数名称(\"sf_\"+fieldName)")]
        //[SsExample("sf_order_no")]
        public string sqlParamName;


    }
}
