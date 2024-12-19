using System;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.ApiTrace;
using Sers.Core.Module.ApiTrace.Collector;
using Sers.Core.Module.App.AppEvent;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Module.Log;

namespace Sers.Gover.Base.AppEvent
{
    public class ApiTracePublisher : IAppEvent
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        Action<Object, Vit.Core.Util.Pipelines.ByteData> ApiScopeEvent(RpcContextData rpcData, ApiMessage apiRequestMessage)
        {

            var traceData = collector.TraceStart(rpcData);


            return (s, apiReplyMessage) =>
            {
                ApiMessage apiResponseMessage = null;

                collector?.TraceEnd(traceData, rpcData, apiRequestMessage, () =>
                {
                    if (apiResponseMessage == null)
                    {
                        apiResponseMessage = new ApiMessage();
                        apiResponseMessage.Unpack(apiReplyMessage.ToArraySegment());
                    }
                    return apiResponseMessage;
                });
            };
        }

        public void InitEvent(JObject arg)
        {
            var collectorName = arg?["collectorName"]?.ToString();
            if (string.IsNullOrEmpty(collectorName))
            {
                Logger.Warn("[LocalApiService.ApiTracePublisher] skip init (can not find collectorName)");
                return;
            }

            ApiTraceMng.InitCollector();

            Logger.Info("[Gover.ApiTracePublisher] initializing");
            if (ApiTraceMng.collectorMap.TryGetValue(collectorName, out collector))
            {
                Logger.Info("[Gover.ApiTracePublisher] initialization successfully", new { collectorName });
            }
            else
            {
                Logger.Error("[Gover.ApiTracePublisher] initialization failure (can not find collector)", new { collectorName = collectorName, Message = "Please configure the correct Sers.ApiTrace.Collector in appsettings.json" });
            }
        }

        IApiTraceCollector collector;

        public void BeforeStart()
        {
            if (collector == null) return;

            GoverApiCenterService.Instance.AddApiScopeEvent(ApiScopeEvent);
            Logger.Info("[Gover.ApiTracePublisher] load successfully");
        }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void OnStart()
        {
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void AfterStart()
        {
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void BeforeStop()
        {
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void AfterStop()
        {
        }
    }



}
