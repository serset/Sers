using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.Api.Rpc;

namespace Sers.Core.Module.Rpc
{
    public class RpcFactory
    {

        public static readonly RpcFactory Instance = new RpcFactory();

        public Func<RpcContext>  CreateRpcContext = 
            () => new RpcContext();
        

        public  Func<IRpcContextData> CreateRpcContextData =
            () => new RpcContextData();
    }
}
