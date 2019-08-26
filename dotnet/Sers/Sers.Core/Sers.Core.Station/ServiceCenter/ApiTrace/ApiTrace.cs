using System;
using Sers.Core.Extensions;
using Sers.Core.Module.Rpc;

namespace Sers.Core.ServiceCenter.ApiTrace
{
    public class ApiTrace : IDisposable
    {
        TraceModel m = new TraceModel();
         

        IRpcContextData rpcData;
        public ApiTrace(IRpcContextData rpcData)
        {
            m.startTime = DateTime.Now;
            this.rpcData = rpcData;

        }

        public void OnException(Exception ex)
        {
        }

        public void Dispose()
        {
            m.endTime = DateTime.Now;
            m.route = rpcData.route;
            m.requestId = rpcData.caller_rid_Get();
            m.parentRequestId = rpcData.caller_parentRequestGuid_Get();
            m.rootRequestId = rpcData.caller_rootRequestGuid_Get();

            m.Publish();
            
        }
    }
}
