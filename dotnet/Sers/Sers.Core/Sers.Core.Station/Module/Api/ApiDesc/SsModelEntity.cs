using System.Collections.Generic;
using Newtonsoft.Json;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace Sers.Core.Module.Api.ApiDesc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SsModelEntity
    {


        /// <summary>
        /// 数据类型。可以唯一定位到一个模型
        /// </summary>
        [JsonProperty]
        [SsExample("int32")]
        [SsDescription("数据类型。可以唯一定位到一个模型")]
        public string type;

        /// <summary>
        ///  数据模式。只可为 value、object、array
        /// </summary>
        [JsonProperty]
        [SsExample("value")]
        [SsDescription("数据模式。只可为 value、object、array")]
        public string mode;



        /// <summary>
        /// 成员属性
        /// </summary>
        [JsonProperty]
        [SsDescription("成员属性")]
        public List<SsModelProperty> propertys;
    }
}
