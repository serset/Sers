using System;
using Sers.Core.Module.Api.Rpc;

namespace Sers.Core.Module.Rpc
{
    public static class RpcFactory
    {
        public static Func<RpcContext> CreateRpcContext  =
            () => new RpcContext();


        public static Func<IRpcContextData> CreateRpcContextData =
            () => new RpcContextData();
    }
}
