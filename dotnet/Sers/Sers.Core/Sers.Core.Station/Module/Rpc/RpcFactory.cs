using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.Api.Rpc;

namespace Sers.Core.Module.Rpc
{
    public class RpcFactory
    {

        public static readonly RpcFactory Instance = new RpcFactory();



        public Func<RpcContext> CreateRpcContext =
            () => new RpcContext();


        public Func<IRpcContextData> CreateRpcContextData =
            () => new RpcContextData();



        #region static OnBegin OnEnd
        internal static List<Func<IDisposable>> RpcContext_OnBegins = null;

        public static void AddRpcContextEvent(Func<IDisposable> ev)
        {
            if (null == ev) return;

            if (null == RpcContext_OnBegins)
            {
                RpcContext_OnBegins = new List<Func<IDisposable>>();
            }
            RpcContext_OnBegins.Add(ev);
        }
        #endregion




    }
}
