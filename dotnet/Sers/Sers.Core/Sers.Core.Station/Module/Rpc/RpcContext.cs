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

        //public static T GetCurrent<T>()
        //    where T : RpcContext
        //{
        //    return Current as T;
        //}

        public static IRpcContextData RpcData => Current?.rpcData;

        #endregion


        #region static OnBegin OnEnd
        static List<Func<IDisposable>> OnBegins=null;

        public static void AddEvent(Func<IDisposable> ev)
        {
            if (null == ev) return;

            if (null == OnBegins)
            {
                OnBegins = new List<Func<IDisposable>>();
            }
            OnBegins.Add(ev);
        }

     
        #endregion



        public ApiMessage apiRequestMessage;
        public ApiMessage apiReplyMessage;

        public IRpcContextData rpcData { get; private set; }
        public void SetRpcContextData(IRpcContextData rpcData)
        {
            this.rpcData = rpcData;
        }






        #region 构造函数 和 Dispose

        private List<IDisposable> onEnds;
        public RpcContext()
        {
            lock (CurrentRpcContext_AsyncCache)
            {
                CurrentRpcContext_AsyncCache.Value = this;
            }

            if (OnBegins != null)
            {
                try
                {
                    onEnds = (from ev in OnBegins
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
