using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
                staticFiles = new StaticFilesConfig { rootPath = rootPath }
            });
        }

        public static void Run(HostRunArg arg)
        {
            Action<IServiceCollection> OnConfigureServices = null;
            Action<IApplicationBuilder> OnConfigure = arg.BeforeConfigure;



            #region #1 AllowAnyOrigin
            if (arg.allowAnyOrigin)
            {
                OnConfigureServices += IServiceCollectionExtensions_AllowAnyOrigin.AllowAnyOrigin_ConfigureServices;
                OnConfigure += IServiceCollectionExtensions_AllowAnyOrigin.AllowAnyOrigin_Configure;
            }
            #endregion

            #region #2 UseStaticFiles
            if (arg.staticFiles != null)
            {
                OnConfigure += (app) =>
                {
                    app.UseStaticFiles(arg.staticFiles);
                };
            }
            #endregion 


            #region #3 Build host

            OnConfigureServices += arg.OnConfigureServices;
            OnConfigure += arg.OnConfigure;

            var host =
                arg.OnCreateWebHostBuilder()
                .UseUrls(arg.urls)
                .ConfigureServices(OnConfigureServices)
                .Configure(OnConfigure)
                .Build();
            #endregion


            #region #4 Run
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