﻿using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Api.ApiEvent.ApiScope;
using Sers.Core.Module.Api.ApiEvent.BeforeCallApi;
using Sers.Core.Module.Api.ApiEvent.BeforeCallApi.AccountInCookie;
using Sers.Core.Module.Api.ApiEvent.BeforeCallApi.Bearer;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Module.Log;
using Vit.Core.Util.Reflection;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.Module.Api.ApiEvent
{
    public class EventBuilder
    {

        #region LoadEvent_BeforeCallApi        
        /// <summary>
        /// 从传入的配置项加载BeforeCallApi事件
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static Action<RpcContextData, ApiMessage> LoadEvent_BeforeCallApi(JArray events)
        {
            if (events == null || events.Count == 0) return null;

            Action<RpcContextData, ApiMessage> BeforeCallApi = null;

            foreach (JObject config in events.Cast<JObject>())
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
                if (className == "Bearer" || className == "Sers.Core.Module.Api.ApiEvent.BeforeCallApi.Bearer.Bearer")
                {
                    return new Bearer();
                }
                if (className == "AccountInCookie" || className == "Sers.Core.Module.Api.ApiEvent.BeforeCallApi.AccountInCookie.AccountInCookie")
                {
                    return new AccountInCookie();
                }
                #endregion


                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile))
                {
                    return null;
                }

                return ObjectLoader.CreateInstance(className, assemblyFile: assemblyFile) as IBeforeCallApi;
            }
            #endregion
        }

        #endregion



        #region LoadEvent_OnCreateScope    
        /// <summary>
        /// 从传入的配置项加载OnCreateScope事件
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IEnumerable<IApiScopeEvent> LoadEvent_OnCreateScope(JArray events)
        {
            if (events == null || events.Count == 0) yield break;

            IApiScopeEvent item;
            foreach (JObject config in events)
            {
                try
                {
                    //(x.x.1) GetInstance
                    item = GetInstance(config);
                    if (item == null) continue;

                    //(x.x.2) init
                    item.Init(config);
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
            IApiScopeEvent GetInstance(JObject config)
            {
                //(x.1) get className    assemblyFile
                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile)) return null;

                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) return null;

                //(x.2)CreateInstance
                return ObjectLoader.CreateInstance(className, assemblyFile: assemblyFile) as IApiScopeEvent;
            }
            #endregion
        }

        #endregion
    }
}
