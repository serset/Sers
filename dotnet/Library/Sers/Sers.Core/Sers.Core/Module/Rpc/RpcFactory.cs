using System;

namespace Sers.Core.Module.Rpc
{
    public static class RpcFactory
    {
        public static Func<RpcContext> CreateRpcContext  =
            () => new RpcContext();

 
    }
}
