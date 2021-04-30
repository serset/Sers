using Sers.Core.Module.Rpc;

namespace Vit.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class RpcContextDataExtensions
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string Bearer_Get(this RpcContextData rpcData)
        {
            //http.headers.Authorization = "Bearer atxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
 

            var Authorization =   (rpcData.http.GetHeader("Authorization") ?? rpcData.http.GetHeader("authorization")) ;
            if (null == Authorization) return null;
            var bear = (Authorization+ " ").Split(' ')[1];           
            return bear;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Bearer_Set(this RpcContextData rpcData, string value)
        {
            if (null == rpcData || string.IsNullOrWhiteSpace(value)) return;
            
            rpcData.http.Headers()["Authorization"] = "Bearer "+value;
        } 
   

    }
}
