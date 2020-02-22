using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
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
            var webHostUrls = ConfigurationManager.Instance.GetByPath<string[]>("Sers.Gateway.WebHost.urls");
            if (webHostUrls == null || webHostUrls.Length == 0) return;


            #region (x.2)初始化GatewayHelp

            var gatewayHelp = new GatewayHelp();

            #region (x.x.1)构建 Api Event BeforeCallApi
            var BeforeCallApi = Sers.Core.Module.Api.ApiEvent.BeforeCallApi.EventBuilder.LoadEvent(ConfigurationManager.Instance.GetByPath<JArray>("Sers.Gateway.BeforeCallApi"));
            if (BeforeCallApi != null) gatewayHelp.BeforeCallApi += BeforeCallApi;
            #endregion


            //(x.x.2)从配置文件加载 服务限流配置
            gatewayHelp.rateLimitMng.LoadFromConfiguration();

            #endregion

            #region (x.3)初始化WebHost

            RunArg arg = new RunArg { allowAnyOrigin = true };



            //(x.x.1)指定可以与iis集成（默认无法与iis集成）
            arg.OnCreateWebHostBuilder = () => Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseVitConfig();

            //(x.x.2)配置web服务监听地址（urls）
            arg.urls = webHostUrls;


            #region (x.x.3)配置静态文件映射

            arg.wwwrootPath = ConfigurationManager.Instance.GetByPath<string>("Sers.Gateway.WebHost.wwwroot");

            if (arg.wwwrootPath != null)
            {
                #region 静态文件类型映射配置（mappings.json）
                try
                {
                    var jsonFile = new JsonFile(new[] { "mappings.json" });
                    if (File.Exists(jsonFile.configPath))
                    {
                        var provider = new FileExtensionContentTypeProvider();
                        var map = provider.Mappings;
                        foreach (var item in (jsonFile.root as JObject))
                        {
                            map.Remove(item.Key);
                            map[item.Key] = item.Value.Value<string>();
                        }

                        arg.OnInitStaticFileOptions += (StaticFileOptions staticfileOptions) =>
                        {
                            staticfileOptions.ContentTypeProvider = provider;
                        };

                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                #endregion
            }
            #endregion


            #region (x.x.4)转发web请求到Sers(网关核心功能)
            arg.OnConfigure = (app) =>
            {
                app.Run(gatewayHelp.Bridge);
            };
            #endregion


            //(x.x.5)设置异步启动
            arg.RunAsync = true;


            #region (x.x.6)启动           
            Logger.Info("[WebHost]will listening on: " + string.Join(",", arg.urls));

            if (arg.wwwrootPath != null)
                Logger.Info("[WebHost]wwwroot : " + arg.wwwrootPath);

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
       
   
            var rpcData = RpcFactory.Instance.CreateRpcContextData().Init(Rpc_CallerSource);

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

  
        string ResponseDefaultContentType = (ConfigurationManager.Instance.GetStringByPath("Sers.Gateway.WebHost.ResponseDefaultContentType") ??( "application/json; charset=" + Vit.Core.Module.Serialization.Serialization.Instance.charset));
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

            //Content-Type → application/json
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
                    return RpcFactory.Instance.CreateRpcContextData().UnpackOriData(rpcContextData_OriData);  
                }
                return null;
            }

            #endregion
        }

        #endregion


       
    }
}
