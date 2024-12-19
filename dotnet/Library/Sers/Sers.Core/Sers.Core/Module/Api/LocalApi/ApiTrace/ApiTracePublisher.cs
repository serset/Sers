using System;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Api.ApiEvent.ApiScope;
using Sers.Core.Module.ApiTrace;
using Sers.Core.Module.ApiTrace.Collector;

using Vit.Core.Module.Log;

namespace Sers.Core.Module.Api.LocalApi.ApiTrace
{
    public class ApiTracePublisher : IApiScopeEvent
    {
        IApiTraceCollector collector;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Init(JObject arg)
        {
            var collectorName = arg?["collectorName"]?.ToString();
            if (string.IsNullOrEmpty(collectorName))
            {
                Logger.Warn("[LocalApiService.ApiTracePublisher] skip init (can not find collectorName)");
                return;
            }

            ApiTraceMng.InitCollector();

            if (ApiTraceMng.collectorMap.TryGetValue(collectorName, out collector))
            {
                Logger.Info("[LocalApiService.ApiTracePublisher] load successfully", new { collectorName });
            }
            else
            {
                Logger.Warn("[LocalApiService.ApiTracePublisher] load failure (can not find collector)", new { collectorName = collectorName, Message = "Please configure the correct Sers.ApiTrace.Collector in appsettings.json" });
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IDisposable OnCreateScope()
        {
            return new ApiTraceItem(collector);
        }
    }
}
