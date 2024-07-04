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
            var rpcData = rpcContext.rpcData;

            // "http://127.0.0.1/Station1/fold1/a/1/2.html?c=9"
            var http_url = rpcData.http.url;
            Uri uri = new Uri(http_url);
            var AbsolutePath = Uri.UnescapeDataString(uri.AbsolutePath);

            var requestFeature = new HttpRequestFeature
            {
                Headers = new HeaderDictionary(),
                Protocol = rpcData.http.protocol,  // "HTTP/2.0"
                Scheme = uri.Scheme,               // "http"
                Method = rpcData.http.method,      // "GET"
                PathBase = "",             // ""
                QueryString = uri.Query,   // ""
                Path = AbsolutePath,       // "/api/values"
                RawTarget = AbsolutePath,  // "/api/values"
            };

            // http header
            if (rpcData.http.headers != null)
            {
                foreach (var t in rpcData.http.headers)
                {
                    requestFeature.Headers[t.Key] = t.Value;
                }
            }

            ArraySegment<byte> arg_OriData = rpcContext.apiRequestMessage.value_OriData;
            requestFeature.Body = new MemoryStream(arg_OriData.Array, arg_OriData.Offset, arg_OriData.Count);

            return requestFeature;
        }



    }
}
