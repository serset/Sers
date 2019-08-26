using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.ApiDiscovery.AspNetCore;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseAspNetCoreApiDiscovery(this LocalApiMng localApiMng, AspNetCoreApiDiscovery.Config config=null)
        {
            if (null == localApiMng)
            {
                return;
            }
            var dis= new AspNetCoreApiDiscovery(localApiMng.apiMap);
            if (null != config)
            {
                dis.config = config;
            }
            localApiMng.discoveryMng.AddDiscovery(dis);
        }



    }
}
