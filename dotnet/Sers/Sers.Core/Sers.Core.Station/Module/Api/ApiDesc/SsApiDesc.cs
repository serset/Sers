using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.ApiDesc;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery.SersValid;

namespace Sers.Core.Module.Api.ApiDesc
{
    /// <summary>
    /// Api描述
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SsApiDesc
    {
        ///// <summary>
        ///// 例如 ApiStation1/path2/api3
        ///// </summary>  
        //[JsonProperty]
        //public string catagory { get; set; }




        /// <summary>
        /// api名称(不为route)
        /// </summary>
        [JsonProperty]
        public string name { get; set; }


        /// <summary>
        /// 文字描述
        /// </summary>
        [JsonProperty]
        public string description { get; set; }

        /// <summary>
        /// 路由 例如 "/ApiStation1/path1/path2/api1"
        /// </summary>        
        [JsonProperty]
        [SsExample("/ApiStation1/path1/path2/api1")]
        public string route { get; set; }

        /// <summary>
        /// 请求参数类型   SsModel类型
        /// </summary>
        [JsonProperty]
        public SsModel argType { get; set; }


        /// <summary>
        /// 返回数据类型   SsModel类型
        /// </summary>
        [JsonProperty]
        public SsModel returnType { get; set; }



        /// <summary>
        /// api调用限制
        /// </summary>
        [JsonProperty]
        public List<SsValidation> rpcValidations { get; set; }

        /// <summary>
        /// api调用限制
        /// </summary>
        [JsonProperty]
        public List<SsValidation> limit { get; set; }


        /// <summary>
        /// 调用api时构建RpcContext 的配置参数
        /// </summary>
        [JsonProperty]
        public JObject rpcContextBuildConfig { get; set; }




        /// <summary>
        /// 额外数据
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object ext { get; set; }


        public string GetApiStationName()
        {
            try
            {
                return route?.Split('/')[1];
            }
            catch (System.Exception)
            {                
            }
            return null;
        }

    }
}
