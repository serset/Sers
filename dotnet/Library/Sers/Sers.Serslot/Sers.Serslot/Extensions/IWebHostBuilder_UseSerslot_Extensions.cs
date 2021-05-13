
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilder_UseSerslot_Extensions
    {
        public static IWebHostBuilder UseSerslot(this IWebHostBuilder hostBuilder)
        {
            if (null == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Sers"))
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
