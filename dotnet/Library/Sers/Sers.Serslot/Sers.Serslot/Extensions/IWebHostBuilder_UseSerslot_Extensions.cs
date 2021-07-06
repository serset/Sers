
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilder_UseSerslot_Extensions
    {
        /// <summary>
        /// 尝试使用Serslot。若未指定配置则不启用Serslot(即appsettings.json未指定Sers节点时不做任何操作)。
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IWebHostBuilder TryUseSerslot(this IWebHostBuilder hostBuilder)
        {
            if (null == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Sers"))
            {
                return hostBuilder;
            }

            return hostBuilder.UseSerslot();
        }

        /// <summary>
        /// 使用Serslot。若未指定配置则使用默认配置强制启用Serslot。
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseSerslot(this IWebHostBuilder hostBuilder)
        {           

            if ("BackgroundTask" == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("serslot.Mode"))
            {
                return hostBuilder.UseSerslot_BackgroundTask();
            }
            else
            {
                return hostBuilder.UseSerslot_Async();
            }
        }


    }
}
