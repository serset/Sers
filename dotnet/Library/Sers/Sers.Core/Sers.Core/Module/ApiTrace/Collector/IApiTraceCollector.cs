using System;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

namespace Sers.Core.Module.ApiTrace.Collector
{
    public interface IApiTraceCollector
    {
        void Init(JObject config);


        object TraceStart(RpcContextData rpcData);


        void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage);
    }
}
