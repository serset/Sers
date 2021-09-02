using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using System;
using System.Runtime.CompilerServices;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.Core.Module.Api.LocalApi.Event
{
    public class LocalApiEvent : IDisposable
    {         

        private IDisposable[] events_OnDispose;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LocalApiEvent(IDisposable[] events_OnDispose)
        {
            this.events_OnDispose = events_OnDispose;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeforeCallApi(RpcContextData rpcData, ApiMessage requestMessage) 
        {
            LocalApiEventMng.Instance.BeforeCallApi?.Invoke(rpcData, requestMessage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (events_OnDispose == null) return;

            events_OnDispose.IEnumerable_ForEach(end =>
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
