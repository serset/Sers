using System;
using System.IO;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http.Features;

using Sers.Core.Module.Rpc;

using Vit.Extensions;
using Vit.Extensions.Json_Extensions;

namespace Sers.Serslot
{
    public partial class SerslotServer
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ProcessRequestByRpc(ArraySegment<byte> arg_OriData)
        {

            HttpRequestFeature requestFeature = null;
            IHttpResponseFeature responseFeature = null;
            try
            {
                var rpcContext = RpcContext.Current;

                //(x.1) build requestFeature
                requestFeature = rpcContext.BuildHttpRequestFeature();

                //(x.2) ProcessRequest
                responseFeature = ProcessRequest(requestFeature);


                #region (x.3)build reply
                var rpcReply = new RpcContextData();

                //(x.x.1)StatusCode
                rpcReply.http.statusCode = responseFeature.StatusCode;

                #region (x.x.2)http_header
                var replyHeader = responseFeature.Headers;
                if (replyHeader != null)
                {
                    var headers = rpcReply.http.Headers();
                    foreach (var item in replyHeader)
                    {
                        headers[item.Key] = item.Value.ToString();
                    }
                }
                #endregion

                //(x.x.3) reply rpcContextData
                rpcContext.apiReplyMessage.rpcContextData_OriData = rpcReply.ToBytes().BytesToArraySegmentByte();


                #region (x.x.4) reply Body
                var body = (responseFeature.Body as MemoryStream).ToArray();
                return body;
                #endregion

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
