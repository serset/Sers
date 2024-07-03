using System;

using Newtonsoft.Json.Linq;

namespace Sers.Core.Module.Api.ApiEvent.ApiScope
{
    /// <summary>
    /// 调用Api时的自定义Scope对象
    /// </summary>
    public interface IApiScopeEvent
    {
        /// <summary>
        /// 在初始化时被调用，会传递配置为config
        /// </summary>
        /// <param name="config"></param>
        void Init(JObject config);


        /// <summary>
        /// 返回对象在apiScope结束时被调用，可为空
        /// </summary>
        /// <returns></returns>
        IDisposable OnCreateScope();
    }
}
