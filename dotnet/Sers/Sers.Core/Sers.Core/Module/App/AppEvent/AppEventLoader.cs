using Newtonsoft.Json.Linq;
using Sers.Core.Module.Reflection;
using System;
using System.Collections.Generic;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.Core.Module.App.AppEvent
{
    public class AppEventLoader
    {

        #region LoadAppEvent    
        /// <summary>
        /// 从传入的配置项加载事件
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IEnumerable<IAppEvent> LoadAppEvent(JArray events)
        {
            if (events == null || events.Count == 0) yield break;

            IAppEvent item;
            foreach (JObject config in events)
            {
                try
                {
                    //(x.x.1) GetInstance
                    item = GetInstance(config);
                    if (item == null) continue;

                    //(x.x.2) init
                    item.InitEvent(config);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    continue;
                }

                //(x.x.3) return
                yield return item;
            }


            #region GetInstance
            IAppEvent GetInstance(JObject config)
            {
                //(x.1) get className    
                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) return null;
                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile)) return null;

                return ObjectLoader.CreateInstance(assemblyFile, className) as IAppEvent;            
            }
            #endregion
        }

        #endregion
    }
}
