using Sers.Core.Module.Rpc;

namespace Vit.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class RpcContextDataExtensions
    {

        public static string Bearer_Get(this RpcContextData rpcData)
        {
            //http.headers.Authorization = "Bearer atxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
 

            var Authorization =   (rpcData.http.GetHeader("Authorization") ?? rpcData.http.GetHeader("authorization")) ;
            if (null == Authorization) return null;
            var bear = (Authorization+ " ").Split(' ')[1];           
            return bear;
        }

        public static void Bearer_Set(this RpcContextData rpcData, string value)
        {
            if (null == rpcData || string.IsNullOrWhiteSpace(value)) return;

            rpcData.http.headers["Authorization"] = "Bearer "+value;
        } 
   

    }
}
