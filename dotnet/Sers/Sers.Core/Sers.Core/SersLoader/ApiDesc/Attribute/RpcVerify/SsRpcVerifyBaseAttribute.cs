using System;
using Newtonsoft.Json.Linq;

namespace Sers.SersLoader.ApiDesc.Attribute.RpcVerify
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class SsRpcVerifyBaseAttribute : System.Attribute
    {
        public abstract void GetApiRpcVerify(JArray rpcVerifySwitchBody);

    }
}
