using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Util.ConfigurationManager;

namespace Sers.ServiceStation.Module.Bearer.Service
{
    public class AccessTokenService
    {
        public static string Api_verifyAt = ConfigurationManager.Instance.GetByPath<string>("Sers.Bearer.Api_verifyAt");
        public static ApiReturn<JObject> VerifyAt(string at)
        { 
            //var arg = "{\"at\":\"" + at + "\"}";
            return ApiClient.CallRemoteApi<ApiReturn<JObject>>(Api_verifyAt, new { at});
        }

    }
}
