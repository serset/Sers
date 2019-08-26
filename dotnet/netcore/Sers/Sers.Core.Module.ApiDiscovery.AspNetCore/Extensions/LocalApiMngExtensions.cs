using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.ApiDiscovery.AspNetCore;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseAspNetCoreApiDiscovery(this LocalApiService localApiService, AspNetCoreApiDiscovery.Config config=null)
        {
            if (null == localApiService)
            {
                return;
            }
            var dis= new AspNetCoreApiDiscovery(localApiService.apiMap);
            if (null != config)
            {
                dis.config = config;
            }
            localApiService.discoveryMng.AddDiscovery(dis);
        }



    }
}
