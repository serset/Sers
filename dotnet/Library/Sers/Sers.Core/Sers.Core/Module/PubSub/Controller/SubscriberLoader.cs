using System;
using System.Linq;
using System.Reflection;

using Vit.Core.Module.Log;

namespace Sers.Core.Module.PubSub.Controller
{
    public class SubscriberLoader
    {
        public static void LoadSubscriber(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(type => typeof(ISubscriberController).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic);
            if (types == null || types.Count() == 0) return;

            Logger.Info("[SubscriberLoader]LoadSubscriber", new { assembly = assembly.FullName });

            foreach (var type in types)
            {
                ISubscriber subscriber = (ISubscriber)Activator.CreateInstance(type);
                SubscriberManage.Instance.Message_Subscribe(subscriber);
            }
        }
    }
}
