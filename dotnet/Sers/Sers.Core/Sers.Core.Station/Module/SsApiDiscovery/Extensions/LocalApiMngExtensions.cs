using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.SsApiDiscovery;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseSsApiDiscovery(this LocalApiService localApiService)
        {
            if (null == localApiService)
            {
                return;
            }

            localApiService.discoveryMng.AddDiscovery(new SsApiDiscovery(localApiService.apiMap));
        }



    }
}
