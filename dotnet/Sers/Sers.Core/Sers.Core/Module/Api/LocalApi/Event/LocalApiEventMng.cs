using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        

        public LocalApiEventMng()
        {
            #region (x.1)构建 Api Event OnCreateScope
            {
                var onCreateScope = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_OnCreateScope(ConfigurationManager.Instance.GetByPath<JArray>("Sers.LocalApiService.OnCreateScope"));
                if (onCreateScope != null) AddEvent_OnCreateScope(onCreateScope);
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
        internal List<Func<IDisposable>> Events_OnCreateScope { get; set; } = null;      
        public void AddEvent_OnCreateScope(Func<IDisposable> ev)
        {
            if (null == ev) return;

            if (null == Events_OnCreateScope)
            {
                Events_OnCreateScope = new List<Func<IDisposable>>();
            }
            Events_OnCreateScope.Add(ev);
        }
        #endregion

        public LocalApiEvent CreateApiEvent()
        {
            List<IDisposable> events_OnDispose =
                Events_OnCreateScope?.Select(event_OnCreateScope =>
                {
                    try
                    {
                        return event_OnCreateScope();
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
