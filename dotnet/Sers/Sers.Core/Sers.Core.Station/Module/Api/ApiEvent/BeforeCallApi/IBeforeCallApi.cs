using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Rpc;

namespace Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi
{
    public interface  IBeforeCallApi
    {
        void Init(JObject config);


        void BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage);
    }
}
