using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.Log;
using Sers.Core.Util.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sers.Core.Module.Rpc
{
    public class RpcContext: IDisposable
    {

        #region static

        static AsyncCache<RpcContext> CurrentRpcContext_AsyncCache = new AsyncCache<RpcContext>();

        public static RpcContext Current => CurrentRpcContext_AsyncCache.Value; 

        public static IRpcContextData RpcData => Current?.rpcData;

        #endregion
                     

        public ApiMessage apiRequestMessage;
        public ApiMessage apiReplyMessage;

        public IRpcContextData rpcData;



        #region 构造函数 和 Dispose

        private List<IDisposable> onEnds;
        public RpcContext()
        {
            //lock (CurrentRpcContext_AsyncCache)
            //{
                CurrentRpcContext_AsyncCache.Value = this;
            //}

            if (RpcFactory.RpcContext_OnBegins != null)
            {
                try
                {
                    onEnds = (from ev in RpcFactory.RpcContext_OnBegins
                              select ev.Invoke()).ToList();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public virtual void Dispose()
        {
            if (onEnds != null)
            {
                onEnds.ForEach(end =>
                {
                    try
                    {
                        end.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                });
                onEnds = null;
            }

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
