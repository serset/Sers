using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

namespace Sers.Core.Module.Api.ApiEvent.BeforeCallApi
{
    public interface  IBeforeCallApi
    {
        void Init(JObject config);


        void BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage);
    }
}
