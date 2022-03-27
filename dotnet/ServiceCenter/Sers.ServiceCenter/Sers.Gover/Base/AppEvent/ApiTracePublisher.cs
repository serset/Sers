using Newtonsoft.Json.Linq;

using Sers.Core.Module.ApiTrace;
using Sers.Core.Module.ApiTrace.Collector;
using Sers.Core.Module.App.AppEvent;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;

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
                Logger.Warn("[Gover.ApiTracePublisher]跳过初始化，没有指定collectorName");
                return;
            }

            ApiTraceMng.InitCollector();
            
            Logger.Info("[Gover.ApiTracePublisher]初始化中");
            if (ApiTraceMng.collectorMap.TryGetValue(collectorName, out collector))
            {
                Logger.Info("[Gover.ApiTracePublisher]已绑定collector", new { collectorName });
            }
            else
            {
                Logger.Error("[Gover.ApiTracePublisher]初始化失败，没有配置指定的collector", new { collectorName = collectorName, Message = "请在appsettings.json中配置正确的Sers.ApiTrace.Collector" });
            }
        }

        IApiTraceCollector collector;

        public void BeforeStart()
        {
            if (collector == null) return;

            GoverApiCenterService.Instance.AddApiScopeEvent(ApiScopeEvent);
            Logger.Info("[Gover.ApiTracePublisher]加载成功");
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
