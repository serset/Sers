using System;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.ApiEvent.ApiScope;
using Sers.Core.Module.Api.LocalApi.Event;
using Vit.Ioc;

namespace Vit.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static class UseIocExtensions
    {
        static bool eventIsAdded = false;
        public static void UseIoc(this LocalApiEventMng data)
        {
            if (eventIsAdded) return;
            eventIsAdded = true;

            data.AddEvent_ApiScope(new ApiScopeEvent());
        }


        class ApiScopeEvent : IApiScopeEvent
        {
            public void Init(JObject config)
            {      
            }

            public IDisposable OnCreateScope()
            {
                return IocHelp.CreateScope();
            }
        }

    }
}
