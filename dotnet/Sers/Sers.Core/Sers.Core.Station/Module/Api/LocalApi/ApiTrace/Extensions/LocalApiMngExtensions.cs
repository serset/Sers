using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Rpc;
using Sers.Core.Station.Module.Api.LocalApi.ApiTrace.Rpc;
using Sers.Core.Util.ConfigurationManager;


namespace Sers.Core.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        public static void UseApiTrace(this LocalApiMng localApiMng)
        {

            if (true != ConfigurationManager.Instance.GetByPath<bool?>("Sers.ApiStation.PrintTrace"))
                return;

            RpcFactory.AddRpcContextEvent(()=> new ApiTraceLog() );


            //RpcFactory.Instance.CreateRpcContext = () => new ServiceStation.ApiTrace.Rpc.RpcContextWithApiTrace();
        }



    }
}
