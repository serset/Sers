using Newtonsoft.Json.Linq;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

namespace Sers.Core.Module.Api.ApiEvent.BeforeCallApi
{
    /// <summary>
    /// 调用api前的事件
    /// </summary>
    public interface IBeforeCallApi
    {
        void Init(JObject config);


        void BeforeCallApi(RpcContextData rpcData, ApiMessage requestMessage);
    }
}
