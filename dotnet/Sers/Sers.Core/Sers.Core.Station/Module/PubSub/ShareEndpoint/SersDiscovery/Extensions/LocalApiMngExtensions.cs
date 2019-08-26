using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.PubSub.ShareEndpoint.SersDiscovery;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseSubscriberDiscovery(this LocalApiMng localApiMng)
        {
            if (null == localApiMng)
            {
                return;                
            }

            localApiMng.discoveryMng.AddDiscovery(new SubscriberDiscovery());
        }



    }
}
