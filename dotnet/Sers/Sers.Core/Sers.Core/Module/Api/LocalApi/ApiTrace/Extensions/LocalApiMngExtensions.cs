using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api.LocalApi.ApiTrace.Rpc;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ConfigurationManager;


namespace Vit.Extensions
{
    public static partial class LocalApiMngExtensions
    {
        /// <summary>
        /// txt log  ("2018-01-01apitrace.log")
        /// </summary>
        /// <param name="localApiMng"></param>
        public static void UseApiTraceLog(this LocalApiService localApiMng)
        {

            if (true != ConfigurationManager.Instance.GetByPath<bool?>("Sers.LocalApiService.PrintTrace"))
                return;

            RpcFactory.AddRpcContextEvent(()=> new ApiTraceLog() );
        }



    }
}
