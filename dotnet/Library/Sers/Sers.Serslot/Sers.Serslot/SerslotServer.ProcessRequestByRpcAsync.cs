using System;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http.Features;

using Sers.Core.Module.Rpc;

using Vit.Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.Serslot
{
    public partial class SerslotServer
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ProcessRequestByRpc(ArraySegment<byte> arg_OriData)
        {
            var rpcContext = RpcContext.Current;

            // #1 build requestFeature
            var requestFeature = rpcContext.BuildHttpRequestFeature();
            using var requestStream = requestFeature.Body;

            // #2 ProcessRequest
            var features = ProcessRequest(requestFeature);
            var bodyFeature = features.Get<IHttpResponseBodyFeature>();
            using var responseStream = bodyFeature.Stream;
            var responseFeature = features.Get<IHttpResponseFeature>();

            #region #3 build reply
            var rpcReply = new RpcContextData();

            // ##1 StatusCode
            rpcReply.http.statusCode = responseFeature.StatusCode;

            #region ##2 http_header
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

            // ##3 reply rpcContextData
            rpcContext.apiReplyMessage.rpcContextData_OriData = rpcReply.ToBytes().BytesToArraySegmentByte();

            // ##4 reply Body
            return responseStream.ToBytes();

            #endregion

        }



    }
}
