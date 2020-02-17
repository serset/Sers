using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Vit.WebHost
{
    public class Host
    {
        #region Run
        public static void Run(int port = 8888, string wwwrootPath = null, Action<IApplicationBuilder> OnConfigure = null)
        {
            Run(wwwrootPath, OnConfigure, "http://*:" + port);
        }
        public static void Run(string wwwrootPath = null, Action<IApplicationBuilder> OnConfigure = null, params string[] urls)
        {
            Run(new RunArg
            {
                wwwrootPath = wwwrootPath,
                OnConfigure = OnConfigure,
                urls = urls
            });
        }

        public static void Run(RunArg arg)
        {
            Action<IServiceCollection> OnConfigureServices = null;
            Action<IApplicationBuilder> OnConfigure = null;



            #region (x.1)允许跨域访问
            if (arg.allowAnyOrigin)
            {
                OnConfigureServices += (services) =>
                {
                    services.AddCors();
                };
                OnConfigure += (app) =>
                {
                    app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                };
            }
            #endregion


            #region (x.2)UseStaticFiles
            if (!string.IsNullOrWhiteSpace(arg.wwwrootPath) || arg.OnInitStaticFileOptions != null)
            {
                OnConfigure += (app) =>
                {
                    var staticfileOptions = new StaticFileOptions();
                    if (!string.IsNullOrWhiteSpace(arg.wwwrootPath))
                    {                          
                        staticfileOptions.FileProvider = new PhysicalFileProvider(arg.wwwrootPath);
                    }
                    arg.OnInitStaticFileOptions?.Invoke(staticfileOptions);
                    app.UseStaticFiles(staticfileOptions);
                };
            }
            #endregion

            #region test OnConfigure
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
                  arg.OnCreateWebHostBuilder().
                   UseUrls(arg.urls).
                   ConfigureServices(OnConfigureServices).
                   Configure(OnConfigure).
                   Build();
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