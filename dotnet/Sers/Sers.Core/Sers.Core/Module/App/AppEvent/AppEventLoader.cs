using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
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
                //(x.x.1) get className    
                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) return null;


                #region (x.x.3) get assembly
                Assembly assembly = null;
   
                #region (x.x.x.1)get assemblyFile Path                
                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile))
                {
                    return null;
                }
                #endregion

                //(x.x.x.2) get assembly from dll file
                assembly = Assembly.LoadFrom(CommonHelp.GetAbsPath(assemblyFile));
 
                #region (x.x.x.3)Get from ReferencedAssemblies               
                if (assembly==null)
                {
                    var assemblyFileName = Path.GetFileNameWithoutExtension(assemblyFile);                     
                    assembly = Assembly.GetEntryAssembly().GetReferencedAssemblies()
                        .Where(m => m.Name == assemblyFileName)
                        .Select(Assembly.Load).FirstOrDefault();
                }
                #endregion

                #endregion

                //(x.x.4) create class
                return assembly?.CreateInstance(className) as IAppEvent;
            }
            #endregion
        }

        #endregion
    }
}
