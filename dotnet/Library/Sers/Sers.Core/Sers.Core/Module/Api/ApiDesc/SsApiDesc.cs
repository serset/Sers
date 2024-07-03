using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Sers.Core.Module.Valid.Sers1;

using Vit.Core.Util.ComponentModel.Model;
using Vit.Core.Util.Extensible;

namespace Sers.Core.Module.Api.ApiDesc
{
    /// <summary>
    /// Api描述
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SsApiDesc : Extensible
    {
        public SsApiDesc Clone()
        {
            return new SsApiDesc
            {
                name = name,
                argType = argType,
                description = description,
                ext = ext,
                returnType = returnType,
                route = route,
                extendConfig = extendConfig?.DeepClone() as JObject,
                rpcValidations = rpcValidations,
                rpcVerify2 = rpcVerify2
            };
        }

        /// <summary>
        /// api名称(不为route)
        /// </summary>
        [JsonProperty]
        [SsExample("getCount")]
        [SsDescription("api名称(不为route)")]
        public String name { get; set; }


        /// <summary>
        /// 文字描述
        /// </summary>
        [JsonProperty]
        [SsExample("获取计数")]
        [SsDescription("文字描述")]
        public String description { get; set; }

        /// <summary>
        /// 路由 例如 "/ApiStation1/path1/path2/api1"
        /// </summary>        
        [JsonProperty]
        [SsExample("/ApiStation1/path1/path2/api1")]
        [SsDescription("路由 例如 \"/ApiStation1/path1/path2/api1\"")]
        public String route { get; set; }



        /// <summary>
        /// 扩展配置（json）
        /// </summary>
        [JsonProperty]
        [SsExample("{\"httpMethod\":\"GET\",\"sysDesc\":\"method: GET\"}")]
        [SsDescription("扩展配置（json）")]
        public JObject extendConfig { get; set; }


        /// <summary>
        /// 请求参数类型(SsModel类型)
        /// </summary>
        [JsonProperty]
        [SsDescription("请求参数类型(SsModel类型)")]
        public SsModel argType { get; set; }


        /// <summary>
        /// 返回数据类型(SsModel类型)
        /// </summary>
        [JsonProperty]
        [SsDescription("返回数据类型(SsModel类型)")]
        public SsModel returnType { get; set; }



        /// <summary>
        /// api调用限制(rpc)，sers1版本使用（为了兼容，暂不禁用）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsDescription("api调用限制(rpc)，sers1版本使用（为了兼容，暂不禁用）")]
        public List<SsValidation> rpcValidations { get; set; }


        /// <summary>
        /// api调用限制(rpc)，sers2版本使用
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsDescription("api调用限制(rpc)，sers2版本使用")]
        public JObject rpcVerify2 { get; set; }

        /// <summary>
        /// 额外数据
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [SsDescription("额外数据")]
        public Object ext { get; set; }






        ///// <summary>
        ///// api调用限制
        ///// </summary>
        //[JsonProperty]
        //public List<SsValidation> limit { get; set; }


        ///// <summary>
        ///// 调用api时构建RpcContext 的配置参数
        ///// </summary>
        //[JsonProperty]
        //public JObject rpcContextBuildConfig { get; set; }

    }
}
