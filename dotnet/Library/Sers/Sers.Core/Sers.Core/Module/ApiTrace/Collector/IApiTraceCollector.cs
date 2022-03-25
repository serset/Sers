using Newtonsoft.Json.Linq;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;

namespace Sers.Core.Module.ApiTrace.Collector
{
    public interface IApiTraceCollector
    {
        void Init(JObject config);

        void AppBeforeStart();

        void AppBeforeStop();




 
        object TraceStart(RpcContextData rpcData);

 
        void TraceEnd(object traceData, RpcContextData rpcData, ApiMessage apiRequestMessage, Func<ApiMessage> GetApiReplyMessage);



    }
}
