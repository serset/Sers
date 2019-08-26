using System;
using Newtonsoft.Json.Linq;

namespace Sers.Core.Module.ApiDesc.Attribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class SsRpcVerifyBaseAttribute : System.Attribute
    {
        public abstract void GetApiRpcVerify(JArray rpcVerifySwitchBody);

    }
}
