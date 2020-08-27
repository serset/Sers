using Newtonsoft.Json;

namespace Sers.Core.Module.Api.LocalApi.StaticFileTransmit.Extensions
{
    #region StaticFilesApiNodeConfig       
    [JsonObject(MemberSerialization.OptIn)]
    public class StaticFilesApiNodeConfig : StaticFilesConfig
    {
        /// <summary>
        /// api路由前缀，例如 "/demo/ui/*"
        /// </summary>
        [JsonProperty]
        public string route { get; set; }

        /// <summary>
        /// api描述，静态文件描述
        /// </summary>
        [JsonProperty]
        public string apiName { get; set; }

    }
    #endregion
}
