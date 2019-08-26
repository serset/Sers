using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Util.ConfigurationManager;
using Sers.ServiceStation.ApiTrace.Rpc;

namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseApiTrace(this LocalApiMng localApiMng)
        {
            if (null == localApiMng)
            {
                return;                
            }


            if (true != ConfigurationManager.Instance.GetByPath<bool?>("Sers.ApiStation.PrintTrace"))
                return;

            RpcFactory.Instance.CreateRpcContext = () => new RpcContextWithApiTrace();
        }



    }
}
