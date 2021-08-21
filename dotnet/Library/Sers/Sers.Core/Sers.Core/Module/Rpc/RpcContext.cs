using System;
using Sers.Core.Module.Message;
using Vit.Core.Util.Threading.Cache;

namespace Sers.Core.Module.Rpc
{
    public class RpcContext : IDisposable
    {

        #region static

        static AsyncCache<RpcContext> CurrentRpcContext_AsyncCache = new AsyncCache<RpcContext>();

        public static RpcContext Current => CurrentRpcContext_AsyncCache.Value;

        public static RpcContextData RpcData => Current?.rpcData;

        #endregion


        public ApiMessage apiRequestMessage;

        public ApiMessage apiReplyMessage;

        public RpcContextData rpcData;



        #region 构造函数 和 Dispose

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public RpcContext()
        {
            //lock (CurrentRpcContext_AsyncCache)
            //{
            CurrentRpcContext_AsyncCache.Value = this;
            //}
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual void Dispose()
        {
            //lock (CurrentRpcContext_AsyncCache)
            //{
            if (CurrentRpcContext_AsyncCache.Value == this)
            {
                CurrentRpcContext_AsyncCache.Value = null;
            }
            //}
        }
        #endregion

    }
}
