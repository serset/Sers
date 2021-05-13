using Microsoft.AspNetCore.Http.Features;
using Sers.Core.Module.Rpc;
using System.IO;
using Sers.Serslot;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Vit.Extensions
{
    public static partial class SerslotServer_ProcessRequest_Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task ProcessRequestByRpcAsync(this SerslotServer data, RpcContext rpcContext)
        {
         
            HttpRequestFeature requestFeature = null;
            IHttpResponseFeature responseFeature = null;
            try
            {
                //(x.1) build requestFeature
                requestFeature = rpcContext.BuildHttpRequestFeature();

                //(x.2) ProcessRequest
                responseFeature = await data.ProcessRequestAsync(requestFeature);         


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
                rpcContext.apiReplyMessage.value_OriData = body.BytesToArraySegmentByte();
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
