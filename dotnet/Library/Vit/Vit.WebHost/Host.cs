using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Linq;

using Vit.Core.Util.ConfigurationManager;
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

            OnConfigureServices += arg.OnConfigureServices ?? (_ => { });
            OnConfigure += arg.OnConfigure ?? (_ => { });

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


        #region BuildContentTypeProvider
        /// <summary>
        /// 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"contentTypeMap.json"。若不指定（或指定的文件不存在）则返回 null
        /// </summary>
        /// <param name="contentTypeMapFile"></param>
        /// <returns></returns>
        public static FileExtensionContentTypeProvider BuildContentTypeProvider(string contentTypeMapFile)
        {
            if (string.IsNullOrWhiteSpace(contentTypeMapFile))
            {
                return null;
            }

            var jsonFile = new JsonFile(contentTypeMapFile);
            if (File.Exists(jsonFile.configPath))
            {
                var provider = new FileExtensionContentTypeProvider();

                if (jsonFile.root is JObject jo)
                {
                    var map = provider.Mappings;
                    foreach (var item in jo)
                    {
                        map.Remove(item.Key);
                        map[item.Key] = item.Value.Value<string>();
                    }
                }
                return provider;
            }

            return null;
        }
        #endregion

    }
}