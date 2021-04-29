using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Vit.Extensions;

namespace Vit.WebHost
{
    public class Host
    {
        #region Run
        public static void Run(int port = 8888, string rootPath = null, Action<IApplicationBuilder> OnConfigure = null)
        {
            Run(rootPath, OnConfigure, "http://*:" + port);
        }
        public static void Run(string rootPath = null, Action<IApplicationBuilder> OnConfigure = null, params string[] urls)
        {
            Run(new HostRunArg
            {                
                OnConfigure = OnConfigure,
                urls = urls,
                staticFiles=new StaticFilesConfig { rootPath= rootPath }
            });
        }

        public static void Run(HostRunArg arg)
        {
            Action<IServiceCollection> OnConfigureServices = null;
            Action<IApplicationBuilder> OnConfigure = null;



            #region (x.1)允许跨域访问
            if (arg.allowAnyOrigin)
            {
                OnConfigureServices += IServiceCollectionExtensions_AllowAnyOrigin.AllowAnyOrigin_ConfigureServices;
                OnConfigure += IServiceCollectionExtensions_AllowAnyOrigin.AllowAnyOrigin_Configure;               
            }
            #endregion

            #region (x.2)UseStaticFiles
            if (arg.staticFiles != null) 
            {
                OnConfigure += (app) =>
                {                   
                    app.UseStaticFiles(arg.staticFiles);
                };
            }
            #endregion

            #region demo OnConfigure
            //OnConfigure += (app) =>{

            //    app.Use(async (context, next) =>
            //    {
            //        await context.Response.WriteAsync("进入第一个委托 执行下一个委托之前\r\n");
            //        //调用管道中的下一个委托
            //        await next.Invoke();
            //        await context.Response.WriteAsync("结束第一个委托 执行下一个委托之后\r\n");
            //    });


            //    app.Run(async (context) =>
            //    {
            //        await context.Response.WriteAsync("hello, here is from netcore.");
            //    });
            //};
            #endregion


            #region (x.3) Build host

            OnConfigureServices += arg.OnConfigureServices;
            OnConfigure += arg.OnConfigure;

            var host =
                arg.OnCreateWebHostBuilder()               
                .UseUrls(arg.urls)    
                .ConfigureServices(OnConfigureServices)
                .Configure(OnConfigure)               
                .Build();
            #endregion


            #region (x.4) Run
            if (arg.RunAsync)
            {
                host.RunAsync();
            }
            else
            {
                host.Run();
            }
            #endregion
        }
        #endregion

    }
}