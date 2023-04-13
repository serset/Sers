using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Api;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Gateway.RateLimit;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;
using Vit.WebHost;

namespace Sers.Gateway
{
    public class GatewayHelp
    {

        #region static Bridge

        public static void Bridge()
        {
            HostRunArg arg = Appsettings.json.GetByPath<HostRunArg>("Sers.Gateway.WebHost");
            if (arg == null || arg.urls == null || arg.urls.Length == 0) return;


            #region (x.2)初始化GatewayHelp

            var gatewayHelp = new GatewayHelp();

            #region (x.x.1)构建 Api Event BeforeCallApi
            var BeforeCallApi = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_BeforeCallApi(Appsettings.json.GetByPath<JArray>("Sers.Gateway.BeforeCallApi"));
            if (BeforeCallApi != null) gatewayHelp.BeforeCallApi += BeforeCallApi;
            #endregion


            //(x.x.2)从配置文件加载 服务限流配置
            gatewayHelp.rateLimitMng.LoadFromConfiguration();

            #endregion


            #region (x.3)初始化WebHost

            //(x.x.1)指定可以与iis集成（默认无法与iis集成）
            arg.OnCreateWebHostBuilder = () =>
                  Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                  .UseVitConfig()
                  .UseCertificates(arg.certificates) //加载https证书（若指定）
                    ;

            //(x.x.2)重定向所有的http请求到https
            if (arg.httpsRedirection != null)
            {
                arg.BeforeConfigure = (app) =>
                {
                    app.UseHttpsRedirection(arg.httpsRedirection);
                };
            }

            #region (x.x.3)转发web请求到Sers(网关核心功能)
            arg.OnConfigure = (app) =>
            {
                app.Run(gatewayHelp.BridgeAsync);
            };
            #endregion


            //(x.x.4)设置异步启动
            arg.RunAsync = true;


            #region (x.x.5)启动
            Logger.Info("[WebHost]listening", arg.urls);

            if (arg.staticFiles?.rootPath != null)
                Logger.Info("[WebHost]wwwroot path",arg.staticFiles.rootPath);

            Vit.WebHost.Host.Run(arg);
            #endregion


            #endregion
        }

        #endregion





        public readonly RateLimitMng rateLimitMng = new RateLimitMng();

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        public Action<RpcContextData, ApiMessage> BeforeCallApi;



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task BridgeAsync(HttpContext context)
        {
            try
            {
                ApiMessage apiReply;

                var error = rateLimitMng.BeforeCall(context);
                if (null != error)
                {
                    apiReply = new ApiMessage().InitAsApiReplyMessageByError(error);
                }
                else
                {
                    apiReply = ApiClient.CallRemoteApi(await BuildApiRequestMessageAsync(context.Request));
                }

                await WriteApiReplyMessage(context.Response, apiReply);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        #region BuildApiRequestMessage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected async Task<ApiMessage> BuildApiRequestMessageAsync(HttpRequest request)
        {
            var rpcData = new RpcContextData().Init(Rpc_CallerSource);

            rpcData.route = request.Path.Value;

            //(x.1)构建http
            BuildHttp(rpcData, request);


            //(x.2) 构建body
            var body = await BuildBodyAsync(request, rpcData);


            #region (x.3) 构建 ApiRequestMessage
            var apiRequestMsg = new ApiMessage
            {
                value_OriData = body.BytesToArraySegmentByte()
            };
            #endregion

            #region (x.4) BeforeCallApi
            try
            {
                BeforeCallApi?.Invoke(rpcData, apiRequestMsg);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            #endregion

            //(x.5) 
            apiRequestMsg.RpcContextData_OriData_Set(rpcData);

            return apiRequestMsg;
        }
        #endregion





        #region BuildHttp
        static string prefixOfCopyIpToHeader = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetStringByPath("Sers.Gateway.WebHost.prefixOfCopyIpToHeader");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void BuildHttp(RpcContextData rpcData, HttpRequest request)
        {
            var http = rpcData.http;

            #region (x.1) url
            http.url = request.GetAbsoluteUri();
            #endregion

            #region (x.2) headers            
            var headers = http.Headers(request.Headers.Count);
            foreach (var kv in request.Headers)
            {
                headers[kv.Key] = kv.Value.ToString();
            }

            //(x.x.2)记录Ip 到 headers
            if (prefixOfCopyIpToHeader != null)
            {
                headers[prefixOfCopyIpToHeader + "RemoteIpAddress"] = request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                headers[prefixOfCopyIpToHeader + "RemotePort"] = "" + request.HttpContext.Connection.RemotePort;

                headers[prefixOfCopyIpToHeader + "LocalIpAddress"] = request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
                headers[prefixOfCopyIpToHeader + "LocalPort"] = "" + request.HttpContext.Connection.LocalPort;
            }
            #endregion

            #region (x.3) method
            http.method = request.Method;
            #endregion

            #region (x.4) protocol
            http.protocol = request.Protocol;
            #endregion


        }

        #endregion

        #region BuildBodyAsync
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task<byte[]> BuildBodyAsync(HttpRequest request, RpcContextData rpcData)
        {
            #region (x.1)二进制数据
            var bytes = await request.Body.ToBytesAsync();
            if (bytes != null && bytes.Length > 0)
            {
                return bytes;
            }
            #endregion


            #region (x.2)从url 构建json参数           
            try
            {
                if (request.Query != null && request.Query.Count != 0)
                {
                    var arg = request.Query.ToDictionary(m => m.Key, m => m.Value.ToString());
                    return arg.SerializeToBytes();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            #endregion
            return null;
        }

        #endregion


        #region WriteApiReplyMessage     

        static string Rpc_CallerSource = Appsettings.json.GetStringByPath("Sers.Gateway.Rpc.CallerSource") ?? "Outside";

        static readonly string Response_ContentType_Json = ("application/json; charset=" + Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance.charset);

        static readonly string ResponseDefaultContentType = Appsettings.json.GetStringByPath("Sers.Gateway.WebHost.ResponseDefaultContentType") ?? Response_ContentType_Json;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task WriteApiReplyMessage(HttpResponse response, ApiMessage apiReply)
        {
            RpcContextData replyRpcData = null;

            #region (x.1)GetReplyRpcData
            {
                var rpcContextData_OriData = apiReply.rpcContextData_OriData;
                if (null != rpcContextData_OriData && rpcContextData_OriData.Count > 0)
                {
                    replyRpcData = RpcContextData.FromBytes(rpcContextData_OriData);
                }
            }
            #endregion


            #region (x.2)statusCode
            var statusCode = replyRpcData?.http.statusCode;
            if (statusCode.HasValue)
            {
                response.StatusCode = statusCode.Value;
            }
            #endregion


            #region (x.3) header
            //(x.x.1)原始header
            var headers = response.Headers;
            if (replyRpcData?.http.headers != null)
            {
                foreach (var item in replyRpcData.http.headers)
                {
                    headers[item.Key] = item.Value;
                }
            }

            //(x.x.2)Content-Type → application/json
            if (!headers.ContainsKey("Content-Type"))
            {
                headers["Content-Type"] = ResponseDefaultContentType;
                //response.ContentType = "application/json";
            }
            #endregion


            //(x.4) Body
            var seg = apiReply.value_OriData;
            if (seg.Array != null && seg.Count > 0)
            {
                await response.Body.WriteAsync(seg.Array, seg.Offset, seg.Count);
            }


        }

        #endregion






    }
}
