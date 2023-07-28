using Sers.Core.Module.Rpc;

namespace Vit.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class RpcContextDataExtensions_Authorization
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string Authorization_Get(this RpcContextData rpcData)
        {
            //http.headers.Authorization = "Bearer atxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            var Authorization = (rpcData.http_header_Get("Authorization") ?? rpcData.http_header_Get("authorization"));
            if (null == Authorization) return null;
            var token = (Authorization + " ").Split(' ')[1];
            return token;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Authorization_Set(this RpcContextData rpcData, string token)
        {
            if (null == rpcData || string.IsNullOrWhiteSpace(token)) return;

            rpcData.http_header_Set("Authorization", "Bearer " + token);
        }


    }
}
