using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.PubSub.SersDiscovery;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseSubscriberDiscovery(this LocalApiService localApiMng)
        {
            if (null == localApiMng)
            {
                return;                
            }

            localApiMng.discoveryMng.AddDiscovery(new SubscriberDiscovery());
        }



    }
}
