using System;
using System.Runtime.CompilerServices;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.Core.Module.Api.LocalApi.Event
{
    public class LocalApiEvent : IDisposable
    {

        private IDisposable[] events_OnDispose;
        private LocalApiEventMng localApiEventMng;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LocalApiEvent(IDisposable[] events_OnDispose, LocalApiEventMng localApiEventMng)
        {
            this.events_OnDispose = events_OnDispose;
            this.localApiEventMng = localApiEventMng;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeforeCallApi(RpcContextData rpcData, ApiMessage requestMessage)
        {
            localApiEventMng?.BeforeCallApi?.Invoke(rpcData, requestMessage);
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
