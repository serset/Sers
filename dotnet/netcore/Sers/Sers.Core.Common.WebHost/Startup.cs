using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;



namespace Sers.Core.Common.WebHost
{
    #region RunArg
    public class RunArg
    {
        /// <summary>
        /// 是否异步运行.Runs a web application and returns a Task that only completes when the token is triggered or shutdown is triggered.
        /// </summary>
        public bool RunAsync = false;

        public string wwwrootPath = null;
        public Action<IApplicationBuilder, IHostingEnvironment> OnConfigure = null;
        public string[] urls;

        /// <summary>
        /// 允许跨域访问
        /// </summary>
        public bool allowAnyOrigin = false;


        public RunArg SetPort(int port = 8888)
        {
            urls = new string[] { "http://*:" + port };
            return this;
        }
    }
    #endregion



    public class Startup
    {
        static Action<IServiceCollection> OnConfigureServices;
        static Action<IApplicationBuilder, IHostingEnvironment> OnConfigure;

        #region Run
       


        public static void Run(int port = 8888, string wwwrootPath = null, Action<IApplicationBuilder, IHostingEnvironment> OnConfigure = null)
        {
            Run(wwwrootPath, OnConfigure, "http://*:" + port);
        }
        public static void Run(string wwwrootPath = null, Action<IApplicationBuilder, IHostingEnvironment> OnConfigure = null, params string[] urls)
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
            Action<IApplicationBuilder, IHostingEnvironment> OnConfigure = null;
            

            if (!string.IsNullOrWhiteSpace(arg.wwwrootPath))
            {
                OnConfigure += (app, env) =>
                {
                    var staticfile = new StaticFileOptions();
                    staticfile.FileProvider = new PhysicalFileProvider(arg.wwwrootPath);
                    app.UseStaticFiles(staticfile);
                };
            }



            #region 允许跨域访问           
            if (arg.allowAnyOrigin)
            {
                OnConfigureServices += (services) => 
                {
                    services.AddCors();
                };
                OnConfigure += (app, env) =>
                {
                    app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                };

            }
            #endregion



            OnConfigure += arg.OnConfigure;




            Startup.OnConfigureServices = OnConfigureServices;
            Startup.OnConfigure = OnConfigure;

            var host =
                   new WebHostBuilder().
                   UseKestrel().
                   //UseContentRoot(wwwrootPath).
                   UseUrls(arg.urls).
                   UseStartup<Startup>().Build();

            if (arg.RunAsync)
            {
                host.RunAsync();
            }
            else
            {
                host.Run();
            }
        }
        #endregion



        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                OnConfigureServices?.Invoke(services);
            }
            catch { }
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
          
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("进入第一个委托 执行下一个委托之前\r\n");
            //    //调用管道中的下一个委托
            //    await next.Invoke();
            //    await context.Response.WriteAsync("结束第一个委托 执行下一个委托之后\r\n");
            //});
            //try
            //{
                OnConfigure?.Invoke(app, env);
            //}
            //catch { }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("hello, here is from netcore.");
            });
        }

       
    }
}