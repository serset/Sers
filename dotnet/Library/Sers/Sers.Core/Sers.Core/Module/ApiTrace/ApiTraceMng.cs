using Newtonsoft.Json.Linq;

using Sers.Core.Module.ApiTrace.Collector;

using System;
using System.Collections.Immutable;

using Vit.Core.Module.Log;
using Vit.Core.Util.Reflection;
using Vit.Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.Module.ApiTrace
{
    public class ApiTraceMng
    {
        public static ImmutableSortedDictionary<string, IApiTraceCollector> collectorMap = ImmutableSortedDictionary<string, IApiTraceCollector>.Empty;

        private static bool inited = false;
        public static void InitCollector()
        {
            if (inited) return;
            inited = true;

            #region GetInstance
            IApiTraceCollector GetInstance(JObject config)
            {
                //(x.x.1) get className
                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) return null;

                #region (x.x.2)是否内置对象
                if (className == "TxtCollector" || className == "Sers.Core.Module.ApiTrace.Collector.TxtCollector")
                {
                    return new TxtCollector();
                }
                #endregion

                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile))
                {
                    return null;
                }
                return ObjectLoader.CreateInstance(className, assemblyFile: assemblyFile) as IApiTraceCollector;
            }
            #endregion


            var configs = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<JObject[]>("Sers.ApiTrace.Collector");
            if (configs == null || configs.Length == 0) return;

            Logger.Info("[ApiTraceMng]collector loading");
            configs.IEnumerable_ForEach(config =>
            {
                try
                {
                    //(x.x.1) GetInstance
                    var collectorName = config["collectorName"]?.ToString();
                    if (string.IsNullOrEmpty(collectorName)) return;
                    if (collectorMap.ContainsKey(collectorName)) return;

                    var collector = GetInstance(config);
                    if (collector == null) return;

                    //(x.x.2) init
                    collector.Init(config);

                    collectorMap = collectorMap.Add(collectorName, collector);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }


            });
            Logger.Info("[ApiTraceMng]collector loaded");

        }

    }
}
