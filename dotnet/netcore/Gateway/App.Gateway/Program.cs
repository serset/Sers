using Microsoft.AspNetCore.Builder;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Common.WebHost;
using Sers.Core.Module.Log;
using System;
using Sers.ServiceStation;
using Sers.Gateway;
using System.IO;
using Sers.Core.Extensions;
using Sers.ServiceStation.Module.Bearer;

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
                ServiceStation.Instance.mqMng.UseZmq();
                #endregion

                //ServiceStation.DiscoveryLocalApi(typeof(Program).Assembly);
                if (!ServiceStation.Start())
                {
                    Logger.Info("无法连接服务中心。站点关闭...");
                    return;
                }

                #endregion


                #region (x.2)初始化GatewayHelp

                var gatewayHelp = new GatewayHelp();

                #region 使用Bearer
                if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.Bearer.UseBearer"))
                {
                    gatewayHelp.BeforeCallApi += BearerHelp.ConvertBearer;
                }       
                #endregion

                //从配置文件加载 服务限流配置
                gatewayHelp.rateLimitMng.LoadFromConfiguration();
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
                    //app.Run(async (context) =>
                    //{
                    //    //await context.Response.WriteAsync("hello, here is from netcore.");
                    //    await gatewayHelp.Bridge(context);
                    //});
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
