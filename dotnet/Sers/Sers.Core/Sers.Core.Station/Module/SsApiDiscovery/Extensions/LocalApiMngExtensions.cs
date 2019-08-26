using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Module.SsApiDiscovery;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseSsApiDiscovery(this LocalApiMng localApiMng)
        {
            if (null == localApiMng)
            {
                return;                
            }

            localApiMng.discoveryMng.AddDiscovery(new SsApiDiscovery(localApiMng.apiMap));
        }



    }
}
