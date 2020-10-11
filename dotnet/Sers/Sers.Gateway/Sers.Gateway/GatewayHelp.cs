using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Gateway.RateLimit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.WebHost;

namespace Sers.Gateway
{
    public class GatewayHelp
    {

        #region static Bridge

        public static void Bridge()
        {
            HostRunArg arg = ConfigurationManager.Instance.GetByPath<HostRunArg>("Sers.Gateway.WebHost"); 
            if (arg == null || arg.urls==null || arg.urls.Length == 0) return;


            #region (x.2)初始化GatewayHelp

            var gatewayHelp = new GatewayHelp();

            #region (x.x.1)构建 Api Event BeforeCallApi
            var BeforeCallApi = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_BeforeCallApi(ConfigurationManager.Instance.GetByPath<JArray>("Sers.Gateway.BeforeCallApi"));
            if (BeforeCallApi != null) gatewayHelp.BeforeCallApi += BeforeCallApi;
            #endregion


            //(x.x.2)从配置文件加载 服务限流配置
            gatewayHelp.rateLimitMng.LoadFromConfiguration();

            #endregion


            #region (x.3)初始化WebHost

            //(x.x.1)指定可以与iis集成（默认无法与iis集成）
            arg.OnCreateWebHostBuilder = () => Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseVitConfig();
            

            #region (x.x.2)转发web请求到Sers(网关核心功能)
            arg.OnConfigure = (app) =>
            {               
                app.Run(gatewayHelp.Bridge);
            };
            #endregion


            //(x.x.3)设置异步启动
            arg.RunAsync = true;


            #region (x.x.4)启动           
            Logger.Info("[WebHost]will listening on: " + string.Join(",", arg.urls));

            if (arg.staticFiles?.rootPath != null)
                Logger.Info("[WebHost]wwwroot : " + arg.staticFiles?.rootPath);

            Vit.WebHost.Host.Run(arg);
            #endregion


            #endregion
        }

        #endregion





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
                    apiReply = new ApiMessage().InitAsApiReplyMessageByError(error);
                }
                else
                {
                    apiReply = ApiClient.CallRemoteApi(BuildApiRequestMessage(context.Request));
                }

                await WriteApiReplyMessage(context.Response, apiReply);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        #region input output


        #region BuildHttp
        static string prefixOfCopyIpToHeader = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetStringByPath("Sers.Gateway.WebHost.prefixOfCopyIpToHeader");
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

            //(x.x.2)记录Ip 到 headers
            if(prefixOfCopyIpToHeader!=null)
            {
                headers[prefixOfCopyIpToHeader+"RemoteIpAddress"] = request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                headers[prefixOfCopyIpToHeader + "RemotePort"] = request.HttpContext.Connection.RemotePort;

                headers[prefixOfCopyIpToHeader + "LocalIpAddress"] = request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
                headers[prefixOfCopyIpToHeader + "LocalPort"] = request.HttpContext.Connection.LocalPort;
            }
            #endregion

            #region (x.3) method
            http["method"] = request.Method;
            #endregion

            #region (x.4) protocol
            http["protocol"] = request.Protocol;
            #endregion         

            return http;
             
        }

        #endregion

        #region BuildBody
        byte[] BuildBody(HttpRequest request, IRpcContextData rpcData)
        {
            #region (x.1)二进制数据
            using (MemoryStream ms = new MemoryStream())
            {
                request.Body.CopyTo(ms);
                if (ms.Length > 0)
                {
                    return ms.ToArray();
                }
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


        static string Rpc_CallerSource = ConfigurationManager.Instance.GetStringByPath("Sers.Gateway.Rpc.CallerSource")?? "Outside";

        protected ApiMessage BuildApiRequestMessage(HttpRequest request)
        {      
   
            var rpcData = RpcFactory.CreateRpcContextData().Init(Rpc_CallerSource);

            rpcData.route = request.Path.Value;
 

            #region (x.1)构建http
            rpcData.http_Set(BuildHttp(request));
            #endregion

            //(x.2) 构建body
            var body = BuildBody(request, rpcData);
 


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

        static readonly string Response_ContentType_Json = ("application/json; charset=" + Vit.Core.Module.Serialization.Serialization.Instance.charset);

        static readonly string ResponseDefaultContentType = ConfigurationManager.Instance.GetStringByPath("Sers.Gateway.WebHost.ResponseDefaultContentType") ?? Response_ContentType_Json;
        async Task WriteApiReplyMessage(HttpResponse response,ApiMessage apiReply)
        {
            var replyRpcData = GetReplyRpcData();

            #region (x.1)statusCode
            var statusCode = replyRpcData?.http_statusCode_Get();
            if (statusCode.HasValue)
            {
                response.StatusCode = statusCode.Value;
            }
            #endregion

         
            #region (x.2) header
            //(x.x.1)原始header
            var headers = response.Headers; 
            if (null != replyRpcData)
            {
                var joHeaders=replyRpcData.http_headers_Get();
                if (null != joHeaders)
                {
                    foreach (var item in joHeaders)
                    {
                        headers[item.Key] = item.Value.ConvertToString();
                    }
                }
            }

            
            //(x.x.2)Content-Type → application/json
            if (!headers.ContainsKey("Content-Type"))
            {
                headers["Content-Type"]= ResponseDefaultContentType;
                //response.ContentType = "application/json";
            }
            #endregion
 

            //(x.3) Body
            var seg = apiReply.value_OriData;
            if (seg.Array != null && seg.Count > 0)
            {
                await response.Body.WriteAsync(seg.Array, seg.Offset, seg.Count);
            }

            #region function GetReplyRpcData
            IRpcContextData GetReplyRpcData()
            {
                var rpcContextData_OriData = apiReply.rpcContextData_OriData;
                if (null != rpcContextData_OriData && rpcContextData_OriData.Count > 0)
                {
                    return RpcFactory.CreateRpcContextData().UnpackOriData(rpcContextData_OriData);  
                }
                return null;
            }

            #endregion
        }

        #endregion


       
    }
}
