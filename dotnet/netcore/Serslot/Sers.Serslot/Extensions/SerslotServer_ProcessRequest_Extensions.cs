using Microsoft.AspNetCore.Http.Features;
using Sers.Core.Module.Rpc;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Sers.Serslot21;

namespace Vit.Extensions
{
    public static partial class SerslotServer_ProcessRequest_Extensions
    {
        public static byte[] ProcessRequestByRpc(this SerslotServer data, ArraySegment<byte> arg_OriData)
        {
            HttpRequestFeature requestFeature = null;
            IHttpResponseFeature responseFeature = null;
            try
            {
                var rpcData = RpcContext.RpcData;

                #region (x.1) build requestFeature
                // "http://127.0.0.1/Station1/fold1/a/1/2.html?c=9"
                var http_url = rpcData.http_url_Get();
                Uri uri = new Uri(http_url);
                //var Query = Uri.UnescapeDataString(uri.Query);
                var Query = uri.Query;
                var AbsolutePath = Uri.UnescapeDataString(uri.AbsolutePath);
                //var AbsolutePath = uri.AbsolutePath;

                requestFeature = new HttpRequestFeature
                {
                    Body = new MemoryStream(arg_OriData.Array, arg_OriData.Offset, arg_OriData.Count),
                    Headers = new HeaderDictionary(),
                    Protocol = rpcData.http_protocol_Get(),
                    Scheme = uri.Scheme,
                    Method = rpcData.http_method_Get(),
                    PathBase = "",
                    QueryString = uri.Query,

                    Path = AbsolutePath,
                    RawTarget = AbsolutePath,

                };

                #region http header
                foreach (var t in rpcData.http_headers_Get())
                {
                    requestFeature.Headers.Add(t.Key, t.Value.ConvertToString());
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


                #region (x.2) ProcessRequest
                responseFeature = data.ProcessRequest(requestFeature);
                #endregion


                #region (x.3)build reply info
                var rpcReply = RpcFactory.Instance.CreateRpcContextData();

                //(x.x.1)StatusCode
                rpcReply.http_statusCode_Set(responseFeature.StatusCode);

                #region (x.x.2)http_header
                var replyHeader = responseFeature.Headers;
                if (replyHeader != null)
                {
                    foreach (var item in replyHeader)
                    {
                        rpcReply.http_header_Set(item.Key, item.Value.ToString());
                    }
                }
                #endregion

                //(x.x.3)
                RpcContext.Current.apiReplyMessage.rpcContextData_OriData = rpcReply.PackageOriData();
                #endregion

                #region (x.4) return reply data
                return (responseFeature.Body as MemoryStream).ToArray();
                #endregion

            }
            finally
            {
                requestFeature?.Body?.Dispose();
                responseFeature?.Body?.Dispose();
            }

        }



    }
}
