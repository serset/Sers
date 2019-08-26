using Newtonsoft.Json;

namespace Sers.Core.Module.Api.ApiDesc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SsModelProperty
    {
        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty]
        public string name;

        /// <summary>
        /// 数据类型。 可为 string、int32、int64、float、double、bool、datetime 或 SsModelEntity的name
        /// </summary>
        [JsonProperty]
        public string type;

        /// <summary>
        ///  数据模式。只可为 value、object、array
        /// </summary>
        [JsonProperty]
        public string mode;

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty]
        public string description;

        /// <summary>
        /// 默认值
        /// </summary>
        [JsonProperty]
        public object defaultValue;

        /// <summary>
        /// 示例值
        /// </summary>
        [JsonProperty]
        public object example;
    }
}
