using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.Serialization;
using Sers.Core.Util.Ioc;
using System;
using System.IO;
using System.Threading.Tasks;
using Sers.Core.Module.Api;
using Sers.Gateway.RateLimit;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sers.Core.Util.ConfigurationManager;

namespace Sers.Gateway
{
    public class GatewayHelp
    {

        public readonly RateLimitMng rateLimitMng = new RateLimitMng();

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        public Action<IRpcContextData, ApiMessage> BeforeCallApi;



        public async Task Bridge(HttpContext context)
        {
            try
            {
                ApiMessage apiReply;

                var error = rateLimitMng.BeforeCall(context);
                if (null != error)
                {
                    apiReply = new ApiMessage().InitByError(error);
                }
                else
                {
                    apiReply = ApiClient.CallRemoteApi(BuildApiRequestMessage(context.Request));
                }

                await WriteApiReplyMessage(context.Response, apiReply);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }


        #region input output


        #region BuildHttp
        protected JObject BuildHttp(HttpRequest request)
        {
            var http = new JObject();

            #region (x.1) url
            http["url"] = request.GetAbsoluteUri();
            #endregion

            #region (x.2) headers
            var headers = http["headers"] = new JObject();
            foreach (var kv in request.Headers)
            {
                headers[kv.Key] = kv.Value.ToString();
            }

            #endregion

            #region (x.3) method
            http["method"] = request.Method;
            #endregion

            return http;
             
        }

        #endregion

        #region BuildArg_AllowBinary
        object BuildArg_AllowBinary(HttpRequest request,IRpcContextData rpcData)
        {

            #region (x.1)二进制数据
            if (rpcData.http_header_Get("Content-Type") == "application/octet-stream")
            {
                var byteData = new List<ArraySegment<byte>>();
                int curLen;
                while (true)
                {
                    var bytes = new byte[102400];
                    curLen = request.Body.Read(bytes, 0, bytes.Length);
                    if (curLen <= 0) break;
                    byteData.Add(new ArraySegment<byte>(bytes, 0, curLen));
                }
                return byteData.ByteDataToBytes();
            }
            #endregion

            //json数据
            return BuildArg_Json(request, rpcData);
        }

        #endregion

        #region BuildArg_Json
        object BuildArg_Json(HttpRequest request, IRpcContextData rpcData)
        {

            #region 构建 json 参数
        
            object argFromBody = null;
            try
            {
                //body       
                string body = null;
                using (var reader = new StreamReader(request.Body))
                {
                    body = reader.ReadToEnd();
                }
                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        argFromBody = JsonConvert.DeserializeObject(body) as JToken;

                        if (null == argFromBody) return body;
                    }
                    catch (Exception ex)
                    {
                        return body;                         
                    }                    
                }
            }
            catch (Exception ex)
            {
                Logger.log.Info(ex);
            }


            
            try
            {
                if (request.Query != null && request.Query.Count != 0)
                {
                    if (argFromBody == null)
                    {
                        argFromBody = new JObject();
                    }

                    if (argFromBody is JObject joArg)
                    {
                        foreach (var item in request.Query)
                        {
                            if (!joArg.ContainsKey(item.Key))
                            {
                                joArg[item.Key] = item.Value.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            #endregion
            return argFromBody;
        }

        #endregion


        static string Rpc_CallerSource = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Sers.Gateway.Rpc.CallerSource")?? "Outside";

        protected ApiMessage BuildApiRequestMessage(HttpRequest request)
        {
            string route = request.Path.ToString();
   
            var rpcData = RpcFactory.Instance.CreateRpcContextData().Init(Rpc_CallerSource);

            rpcData.route = route;
 

            #region (x.1)构建http
            rpcData.http_Set(BuildHttp(request));
            #endregion

            //(x.2) 构建arg
            var arg = BuildArg_AllowBinary(request, rpcData);
            //var arg = BuildArg_Json(request, rpcData);


            #region (x.3) 构建 ApiRequestMessage
            var apiRequestMsg = new ApiMessage
            {
                value_OriData = Serialization.Instance.Serialize(arg).BytesToArraySegmentByte()
            };
            #endregion

            #region (x.4) BeforeCallApi
            try
            {
                BeforeCallApi?.Invoke(rpcData, apiRequestMsg);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            #endregion

            //(x.5) 
            apiRequestMsg.RpcContextData_OriData_Set(rpcData);

            return apiRequestMsg;
        }

  
        string ResponseDefaultContentType = (ConfigurationManager.Instance.GetStringByPath("Sers.Gateway.WebHost.ResponseDefaultContentType") ??( "application/json; charset=" + Sers.Core.Module.Serialization.Serialization.Instance.charset));
        async Task WriteApiReplyMessage(HttpResponse response,ApiMessage apiReply)
        {
            //header
            #region (x.1) header             
            var headers = response.Headers;
            var rpcContextData_OriData=apiReply.rpcContextData_OriData;
            if (null != rpcContextData_OriData && rpcContextData_OriData.Count > 0)
            {
                var replyRpcData = RpcFactory.Instance.CreateRpcContextData();
                replyRpcData.UnpackOriData(rpcContextData_OriData);

                var joHeaders=replyRpcData.http_headers_Get();

                if (null != joHeaders)
                {
                    foreach (var item in joHeaders)
                    {
                        headers[item.Key] = item.Value.ConvertToString();
                    }
                }
            }

            //Content-Type → application/json
            if (!headers.ContainsKey("Content-Type"))
            {
                headers["Content-Type"]= ResponseDefaultContentType;
                //response.ContentType = "application/json";
            }
            #endregion


            //(x.2) Body
            var seg = apiReply.value_OriData;
            await response.Body.WriteAsync(seg.Array, seg.Offset,seg.Count);
        }

        #endregion


       
    }
}
