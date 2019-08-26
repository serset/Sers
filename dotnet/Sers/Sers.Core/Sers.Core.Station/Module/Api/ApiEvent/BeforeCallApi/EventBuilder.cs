using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Sers.Core.Util.Common;
using System;
using System.Reflection;

namespace Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi
{
    public class EventBuilder
    {
        public static Action<IRpcContextData, ApiMessage> LoadEvent(JArray events)
        {
            Action<IRpcContextData, ApiMessage> BeforeCallApi = null;

            if (events == null || events.Count == 0) return BeforeCallApi;

            foreach (JObject config in events)
            {
                try
                {
                    //(x.x.1) GetInstance
                    var item = GetInstance(config);
                    if (item == null) continue;

                    //(x.x.2) init
                    item.Init(config);

                    //(x.x.3) add event
                    BeforeCallApi += item.BeforeCallApi;

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return BeforeCallApi;

            #region GetInstance
            IBeforeCallApi GetInstance(JObject config)
            {
                //(x.x.1) get className    
                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) return null;

                #region (x.x.2)是否内置对象
                if (className == "Bearer" || className == "Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi.Bearer.Bearer")
                {
                    return new Bearer.Bearer();
                }
                if (className == "AccountInCookie" || className == "Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi.AccountInCookie.AccountInCookie")
                {
                    return new AccountInCookie.AccountInCookie();
                }
                #endregion


                #region (x.x.3) get assembly
                Assembly assembly = null;
                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile))
                {
                    return null;
                }
                assembly = Assembly.LoadFrom(CommonHelp.GetAbsPathByRealativePath(assemblyFile));
                #endregion

                //(x.x.4) create class
                return assembly?.CreateInstance(className) as IBeforeCallApi;
            }
            #endregion
        }


    }
}
