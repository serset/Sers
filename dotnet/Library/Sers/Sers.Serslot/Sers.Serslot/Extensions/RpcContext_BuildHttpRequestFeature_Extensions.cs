using System;
using System.IO;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

using Sers.Core.Module.Rpc;

namespace Vit.Extensions
{
    internal static partial class RpcContext_BuildHttpRequestFeature_Extensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HttpRequestFeature BuildHttpRequestFeature(this RpcContext rpcContext)
        {
            HttpRequestFeature requestFeature;

            #region build requestFeature

            ArraySegment<byte> arg_OriData = rpcContext.apiRequestMessage.value_OriData;
            var rpcData = rpcContext.rpcData;

            // "http://127.0.0.1/Station1/fold1/a/1/2.html?c=9"
            var http_url = rpcData.http.url;
            Uri uri = new Uri(http_url);
            var AbsolutePath = Uri.UnescapeDataString(uri.AbsolutePath);

            requestFeature = new HttpRequestFeature
            {
                Body = new MemoryStream(arg_OriData.Array, arg_OriData.Offset, arg_OriData.Count),
                Headers = new HeaderDictionary(),
                Protocol = rpcData.http.protocol,
                Scheme = uri.Scheme,
                Method = rpcData.http.method,
                PathBase = "",
                QueryString = uri.Query,

                Path = AbsolutePath,
                RawTarget = AbsolutePath,
            };

            #region http header
            if (rpcData.http.headers != null)
                foreach (var t in rpcData.http.headers)
                {
                    requestFeature.Headers[t.Key] = t.Value;
                }
            #endregion

            //var requestFeature = new HttpRequestFeature
            //{
            //    Body = new MemoryStream(),
            //    Protocol = "HTTP/2.0",
            //    Scheme = "http",
            //    Method = "GET",
            //    PathBase = "",
            //    Path = "/api/values",
            //    QueryString = "",
            //    RawTarget = "/api/values"
            //};
            #endregion

            return requestFeature;
        }



    }
}
