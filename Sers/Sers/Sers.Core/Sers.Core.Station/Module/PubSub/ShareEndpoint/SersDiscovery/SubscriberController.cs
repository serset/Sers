using Sers.Core.Module.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.PubSub.ShareEndpoint.SersDiscovery
{
    public abstract class SubscriberController<T> : ISubscriberController
    {
        public SubscriberController(string msgTitle)
        {
            this.msgTitle= msgTitle;
        }

        public string msgTitle { get; protected set; }


        public void OnMessage(ArraySegment<byte> msgBody)
        {
            T t;
            try
            {
                t=Serialization.Serialization.Instance.Deserialize<T>(msgBody);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                return;
                t = default(T);
            }

            try
            {
                Handle(t);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }

        }


        public abstract void Handle(T msgBody);

      
    }
}
