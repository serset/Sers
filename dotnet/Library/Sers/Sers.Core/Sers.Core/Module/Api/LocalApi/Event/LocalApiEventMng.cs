using Newtonsoft.Json.Linq;

using Sers.Core.Module.Api.ApiEvent.ApiScope;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Vit.Core.Module.Log;

namespace Sers.Core.Module.Api.LocalApi.Event
{
    /// <summary>
    /// 事件顺序为  OnCreateScope -> BeforeCallApi -> OnDispose
    /// </summary>
    public class LocalApiEventMng
    {

        public void Init(JToken config)
        {
            if (config == null) return;

            #region (x.1)构建 Api Event OnCreateScope
            {
                var apiScopeEvents = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_OnCreateScope(config["OnCreateScope"] as JArray);
                AddEvent_ApiScope(apiScopeEvents.ToArray());
            }
            #endregion

            #region (x.2)构建 Api Event BeforeCallApi
            {
                var beforeCallApi = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_BeforeCallApi(config["BeforeCallApi"] as JArray);
                if (beforeCallApi != null) BeforeCallApi += beforeCallApi;
            }
            #endregion
        }


        #region OnCreateScope
        internal List<IApiScopeEvent> apiScopeEventList = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEvent_ApiScope(params IApiScopeEvent[] apiScopeEvents)
        {
            if (null == apiScopeEvents || apiScopeEvents.Length == 0) return;

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
            if (apiScopeEventList == null) return null;

            var events_OnDispose =
                apiScopeEventList.Select(apiScope =>
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
                .ToArray();

            return new LocalApiEvent(events_OnDispose, this);
        }


        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        internal Action<RpcContextData, ApiMessage> BeforeCallApi = null;

    }
}
