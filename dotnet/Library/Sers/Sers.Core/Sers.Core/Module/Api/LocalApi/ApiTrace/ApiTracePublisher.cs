using Newtonsoft.Json.Linq;

using Sers.Core.Module.Api.ApiEvent.ApiScope;
using Sers.Core.Module.ApiTrace;
using Sers.Core.Module.ApiTrace.Collector;

using System;

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
                Logger.Warn("[LocalApiService.ApiTracePublisher]跳过初始化，没有指定collectorName");
                return;
            }

            ApiTraceMng.InitCollector();

            if (ApiTraceMng.collectorMap.TryGetValue(collectorName, out collector))
            {
                Logger.Info("[LocalApiService.ApiTracePublisher]加载成功", new { collectorName });
            }
            else
            {
                Logger.Warn("[LocalApiService.ApiTracePublisher]加载失败，没有配置指定的collector", new { collectorName = collectorName, Message = "请在appsettings.json中配置正确的Sers.ApiTrace.Collector" });
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IDisposable OnCreateScope()
        {
            return new ApiTraceItem(collector);
        }
    }
}
