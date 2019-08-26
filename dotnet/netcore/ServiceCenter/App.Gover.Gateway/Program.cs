using Microsoft.AspNetCore.Builder;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Common.WebHost;
using Sers.Core.Module.Log;
using System;
using Sers.ServiceStation;
using System.IO;
using Sers.Core.Extensions;
using Sers.Core.Module.Rpc;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.Module.Api.Message;
using Sers.ServiceStation.Module.Bearer;
using Sers.Gateway;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            { 

                #region (x.1)初始化ServiceStation                
                ServiceStation.Init();

                #region 使用扩展消息队列            
                //ServiceStation.Instance.mqMng.UseZmq();
                #endregion

                //ServiceStation.Discovery(typeof(Program).Assembly);
                if (!ServiceStation.Start())
                {
                    Logger.Info("无法连接服务中心。站点关闭...");
                    return;
                }

                #endregion


                #region (x.2)初始化GatewayHelp

                var gatewayHelp = new GatewayHelp();


                #region (x.1)使用Bearer
                if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.Bearer.UseBearer"))
                {
                    gatewayHelp.BeforeCallApi += BearerHelp.ConvertBearer;
                }
                #endregion

                #region (x.2) Gove 权限校验
                string authUser = ConfigurationManager.Instance.GetByPath<string>("Sers.Gateway.Auth.user");

                //设置Api调用标识（GoverGateway）
                gatewayHelp.BeforeCallApi += (IRpcContextData rpcData, ApiMessage requestMessage) => 
                {
                    #region (x.1)权限校验
                    if (String.IsNullOrEmpty(authUser))
                    {
                        #region (x.x.1)无需权限校验
                        rpcData.caller_source_Set("Internal");
                        #endregion
                    }
                    else
                    {
                        #region (x.x.2)通过cookie权限校验

                        string authUserFromCookie=null;

                        #region (x.x.x.1)获取cookie 中的参数
                        var cookie = rpcData.http_header_Get("Cookie");
                        if (!string.IsNullOrEmpty(cookie))
                        {
                            try
                            {
                                // "a=b;c=7"
                                string str = cookie;
                                char entrySeparator = ';';
                                char kvSeparator = '=';
                                Dictionary<string, string> dictionary;
                                {
                                    //dictionary = str.Split(new string[] { entrySeparator }, StringSplitOptions.RemoveEmptyEntries)
                                    //    .GroupBy(x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[0], x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[1])
                                    //    .ToDictionary(x => x.Key, x => x.First());
                                    dictionary = str.Split(new[] { entrySeparator }, StringSplitOptions.RemoveEmptyEntries)
                                       //.GroupBy(x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[0], x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[1])
                                       .Select(x => x.Split(new[] { kvSeparator })).ToDictionary(kv => kv[0]?.Trim(), kv => kv[1]?.Trim());
                                }
                                authUserFromCookie = dictionary.ContainsKey("user") ? dictionary["user"] : null;                               
                            }
                            catch (Exception)
                            {
                            }
                        }
                        #endregion

                        #region (x.x.x.2)权限验证
                        if (authUserFromCookie == authUser)
                        {
                            rpcData.caller_source_Set("Internal");
                        }
                        else
                        {
                            rpcData.caller_source_Set("GoverGateway");
                        }
                        #endregion

                        #endregion
                    }
                    #endregion

                };
                #endregion

                #endregion

                #region (x.3)初始化WebHost

                RunArg arg = new RunArg { allowAnyOrigin=true};

                arg.urls = ConfigurationManager.Instance.GetByPath<string[]>("Sers.Gateway.WebHost.urls");

                var wwwroot = ConfigurationManager.Instance.GetByPath<string>("Sers.Gateway.WebHost.wwwroot");
                if (""== wwwroot)
                    wwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

                if (Directory.Exists(wwwroot))
                {
                    arg.wwwrootPath = wwwroot;
                }

                arg.OnConfigure = (app, env) => {
                    app.Run(gatewayHelp.Bridge);
                };
                arg.RunAsync = true;
                Sers.Core.Common.WebHost.Startup.Run(arg);
                #endregion


                ServiceStation.RunAwait();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Exception:"+ex.Message);
            }
          
 
        }
    }
}
