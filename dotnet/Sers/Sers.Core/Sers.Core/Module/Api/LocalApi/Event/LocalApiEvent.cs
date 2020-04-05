using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using System;
using System.Collections.Generic;
using Vit.Core.Module.Log;

namespace Sers.Core.Module.Api.LocalApi.Event
{
    public class LocalApiEvent : IDisposable
    {         

        private List<IDisposable> events_OnDispose;

        internal LocalApiEvent(List<IDisposable> events_OnDispose)
        {
            this.events_OnDispose = events_OnDispose;
        }
       

        public void BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage) 
        {
            LocalApiEventMng.Instance.BeforeCallApi?.Invoke(rpcData, requestMessage);
        }

        public virtual void Dispose()
        {
            if (events_OnDispose == null) return;

            events_OnDispose.ForEach(end =>
            {
                try
                {
                    end?.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
            events_OnDispose = null;
        }

    }
}
