using Vit.Core.Module.Log;
using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.ApiTrace.Collector;

namespace Sers.Core.Module.Api.LocalApi.ApiTrace
{
    class ApiTraceItem : IDisposable
    {
        IApiTraceCollector collector;
        object traceData;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ApiTraceItem(IApiTraceCollector collector)
        {
            this.collector = collector;
            traceData = collector.TraceStart(RpcContext.Current.rpcData);
        }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            try
            {
                var rpcContext = RpcContext.Current;
                collector?.TraceEnd(traceData, rpcContext.rpcData, rpcContext.apiRequestMessage, () => rpcContext.apiReplyMessage);
                collector = null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
