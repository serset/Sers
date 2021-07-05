
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilder_UseSerslot_Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="useDefaultConfig">在不指定配置时是否使用默认配置强制启用Serslot。若为false,且appsettings.json不指定Sers则不做任何操作</param>
        /// <returns></returns>
        public static IWebHostBuilder UseSerslot(this IWebHostBuilder hostBuilder, bool useDefaultConfig = false)
        {
            if (!useDefaultConfig && null == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Sers"))
            {
                return hostBuilder;
            }

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
