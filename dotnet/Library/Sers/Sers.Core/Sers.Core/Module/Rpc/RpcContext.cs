using System;

using Sers.Core.Module.Message;

using Vit.Core.Util.Threading.Cache;

namespace Sers.Core.Module.Rpc
{
    public class RpcContext : AsyncCacheScope<RpcContext>
    {

        #region static
        public static RpcContext Current => Instance;

        public static RpcContextData RpcData => Current?.rpcData;

        #endregion


        public ApiMessage apiRequestMessage;

        public ApiMessage apiReplyMessage;

        public RpcContextData rpcData;

    }
}
