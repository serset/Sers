using Sers.Core.Module.SersDiscovery;
using System;
using System.Linq;

namespace Sers.Core.Module.PubSub.SersDiscovery
{
    public class SubscriberDiscovery : ISersDiscovery
    {


        public void Discovery(DiscoveryConfig config)
        {
            var types = config.assembly.GetTypes().Where(type => typeof(ISubscriberController).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic);
            foreach (var type in types)
            {
                ISubscriber subscriber = (ISubscriber)Activator.CreateInstance(type);
                EndpointManage.Instance.Message_Subscribe(subscriber);
            }
        }
    }
}
