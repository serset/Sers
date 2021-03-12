using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.ApiEvent.ApiScope;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;

namespace Sers.Core.Module.Api.LocalApi.Event
{
    /// <summary>
    /// 事件顺序为  OnCreateScope -> BeforeCallApi -> OnDispose
    /// </summary>
    public class LocalApiEventMng
    {

        public static readonly LocalApiEventMng Instance  = new LocalApiEventMng();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LocalApiEventMng()
        {
            #region (x.1)构建 Api Event OnCreateScope
            {
                var apiScopeEvents = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_OnCreateScope(ConfigurationManager.Instance.GetByPath<JArray>("Sers.LocalApiService.OnCreateScope"));

                 AddEvent_ApiScope(apiScopeEvents.ToArray());
            }
            #endregion

            #region (x.2)构建 Api Event BeforeCallApi
            {
                var beforeCallApi = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_BeforeCallApi(ConfigurationManager.Instance.GetByPath<JArray>("Sers.LocalApiService.BeforeCallApi"));
                if (beforeCallApi != null) BeforeCallApi += beforeCallApi;
            }
            #endregion
        }



        #region OnCreateScope
        internal List<IApiScopeEvent> apiScopeEventList = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEvent_ApiScope( params IApiScopeEvent[] apiScopeEvents)
        {
            if (null == apiScopeEvents) return;

            if (null == apiScopeEventList)
            {
                apiScopeEventList = new List<IApiScopeEvent>();
            }
            apiScopeEventList.AddRange(apiScopeEvents);
        }
        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LocalApiEvent CreateApiEvent()
        {
            List<IDisposable> events_OnDispose =
                apiScopeEventList?.Select(apiScope =>
                {
                    try
                    {
                        return apiScope.OnCreateScope();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                    return null;
                })
                .ToList();

            return new LocalApiEvent(events_OnDispose);
        }


        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        internal Action<IRpcContextData, ApiMessage> BeforeCallApi = null;


    }
}
