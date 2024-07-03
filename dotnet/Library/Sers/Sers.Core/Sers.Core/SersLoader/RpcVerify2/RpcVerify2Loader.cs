using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Linq;

using Sers.SersLoader.ApiDesc.Attribute.RpcVerify;

namespace Sers.SersLoader.RpcVerify2
{
    public class RpcVerify2Loader
    {
        public static JObject GetRpcVerify2FromMethod(MethodInfo info)
        {
            var attrs = info.GetCustomAttributes<SsRpcVerifyBaseAttribute>();

            if (attrs.Count() == 0) return null;

            var switchBody = new JArray();
            var rpcVerify2 = new JObject
            {
                ["type"] = "Switch",
                ["body"] = switchBody
            };

            foreach (var attr in attrs)
            {
                attr.GetApiRpcVerify(switchBody);
            }

            if (switchBody.Count == 0) return null;

            return rpcVerify2;
        }










    }
}
