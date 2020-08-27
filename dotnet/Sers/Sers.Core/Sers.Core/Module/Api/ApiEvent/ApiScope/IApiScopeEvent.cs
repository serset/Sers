using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using System;

namespace Sers.Core.Module.Api.ApiEvent.ApiScope
{
    /// <summary>
    /// 调用Api时的自定义Scope对象
    /// </summary>
    public interface  IApiScopeEvent
    {
        void Init(JObject config);

   
        /// <summary>
        /// 返回对象在apiScope结束时被调用，可为空
        /// </summary>
        /// <returns></returns>
        IDisposable OnCreateScope();
    }
}
