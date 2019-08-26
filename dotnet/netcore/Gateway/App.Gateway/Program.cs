using Microsoft.AspNetCore.Builder;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Common.WebHost;
using Sers.Core.Module.Log;
using System;
using Sers.ServiceStation;
using System.IO;
using Sers.Gateway;
using Newtonsoft.Json.Linq;

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

                //ServiceStation.Discovery(typeof(Program).Assembly);
                if (!ServiceStation.Start())
                {
                    Logger.Info("无法连接服务中心。站点关闭...");
                    return;
                }

                #endregion


                #region (x.2)初始化GatewayHelp

                var gatewayHelp = new GatewayHelp();

                #region (x.x.1)构建 Api Event BeforeCallApi
                var BeforeCallApi = Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi.EventBuilder.LoadEvent(ConfigurationManager.Instance.GetByPath<JArray>("Sers.Api.BeforeCallApi"));
                if (BeforeCallApi != null) gatewayHelp.BeforeCallApi += BeforeCallApi;
                #endregion
                 

                //(x.x.2)从配置文件加载 服务限流配置
                gatewayHelp.rateLimitMng.LoadFromConfiguration();

                #endregion

                #region (x.3)初始化WebHost

                RunArg arg = new RunArg { allowAnyOrigin = true };

                arg.urls = ConfigurationManager.Instance.GetByPath<string[]>("Sers.Gateway.WebHost.urls");

                var wwwroot = ConfigurationManager.Instance.GetByPath<string>("Sers.Gateway.WebHost.wwwroot");
                if ("" == wwwroot)
                    wwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

                if (Directory.Exists(wwwroot))
                {
                    arg.wwwrootPath = wwwroot;
                }

                arg.OnConfigure = (app, env) =>
                {
                    app.Run(gatewayHelp.Bridge);
                };
                arg.RunAsync = true;

                Logger.Info("[WebHost]will listening on: " + string.Join(',', arg.urls));
                Logger.Info("[WebHost]wwwroot : " + wwwroot);

                Sers.Core.Common.WebHost.Startup.Run(arg);
                #endregion


                ServiceStation.RunAwait();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine("Exception:" + ex.Message);
            }


        }
    }
}
