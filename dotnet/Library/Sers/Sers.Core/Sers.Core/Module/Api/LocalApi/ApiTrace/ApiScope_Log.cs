using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.ApiEvent.ApiScope;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Api.LocalApi.ApiTrace
{
    class ApiScope_Log : IApiScopeEvent
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Init(JObject config)
        {       
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IDisposable OnCreateScope()
        {
            return new ApiTraceLog();
        }
    }
}
