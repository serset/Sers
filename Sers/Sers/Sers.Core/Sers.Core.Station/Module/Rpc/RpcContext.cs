using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Util.Threading;
using System;

namespace Sers.Core.Module.Rpc
{
    public class RpcContext: IDisposable
    {

        #region static

        static AsyncCache<RpcContext> CurrentRpcContext_AsyncCache = new AsyncCache<RpcContext>();

        public static RpcContext Current => CurrentRpcContext_AsyncCache.Value;

        //public static T GetCurrent<T>()
        //    where T : RpcContext
        //{
        //    return Current as T;
        //}

        public static IRpcContextData RpcData => Current?.rpcData;

        #endregion




        public ApiMessage apiRequestMessage;
        public ApiMessage apiReplyMessage;

        public IRpcContextData rpcData { get; private set; }
        public void SetRpcContextData(IRpcContextData rpcData)
        {
            this.rpcData = rpcData;
        }


        #region 构造函数 和 Dispose


        public RpcContext()
        {
            lock (CurrentRpcContext_AsyncCache)
            {
                CurrentRpcContext_AsyncCache.Value = this;
            }
        }

        public virtual void Dispose()
        {    
            lock (CurrentRpcContext_AsyncCache)
            {
                if (CurrentRpcContext_AsyncCache.Value == this)
                {
                    CurrentRpcContext_AsyncCache.Value = null;
                }
            }
        }
        #endregion

    }
}
