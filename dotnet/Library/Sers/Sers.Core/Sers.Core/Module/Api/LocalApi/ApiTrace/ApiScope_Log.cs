using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.ApiEvent.ApiScope;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Api.LocalApi.ApiTrace
{
    class ApiScope_Log : IApiScopeEvent
    {
        public void Init(JObject config)
        {       
        }

        public IDisposable OnCreateScope()
        {
            return new ApiTraceLog();
        }
    }
}
