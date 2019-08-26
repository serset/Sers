using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sers.Core.Module.Api.ApiDesc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SsModelEntity
    {


        /// <summary>
        /// 数据类型。可以唯一定位到一个模型
        /// </summary>
        [JsonProperty]
        public string type;

        /// <summary>
        ///  数据模式。只可为 value、object、array
        /// </summary>
        [JsonProperty]
        public string mode;

 


        [JsonProperty]
        public List<SsModelProperty> propertys;
    }
}
