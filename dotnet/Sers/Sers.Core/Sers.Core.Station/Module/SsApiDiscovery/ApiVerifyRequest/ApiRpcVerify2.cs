using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.Core.Util.SsError;
using Sers.Core.Util.SsExp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Sers.Core.Station.Module.SsApiDiscovery.ApiVerifyRequest
{
    public class ApiRpcVerify2
    {
        public static JObject GetRpcVerify2FromMethod(MethodInfo info)
        {
            var attrs = info.GetCustomAttributes<SsRpcVerifyBaseAttribute>();

            if (attrs.Count() == 0) return null;

            var switchBody = new JArray();
            var rpcVerify2 = new JObject {
                ["type"]= "Switch",
                ["body"] = switchBody
            };
          
            foreach (var attr in attrs)
            {
                attr.GetApiRpcVerify(switchBody);
            }

            if (switchBody.Count == 0) return null;

            return rpcVerify2;
        }




        public static bool Verify(JObject rpcData, JObject ssExp, out SsError ssError)
        {
            ssError = null;
            if (ssExp.IsNull()) return true;

            var varifyResult = SsExpCalculator.Calculate(rpcData, ssExp);
            if (varifyResult.IsJObject())
            {
                ssError = varifyResult.Deserialize<SsError>();              
                return false;
            }
            return true;
        }





    }
}
