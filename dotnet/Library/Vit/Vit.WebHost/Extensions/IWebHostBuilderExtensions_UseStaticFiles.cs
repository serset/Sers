using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IWebHostBuilderExtensions_UseStaticFiles
    {
        /// <summary>
        /// 启用静态文件服务
        /// </summary>
        /// <param name="data"></param>
        /// <param name="configPath">在appsettings.json文件中的路径。默认:"server.staticFiles"。</param>
        /// <returns></returns>
        public static IWebHostBuilder UseStaticFilesFromConfig(this IWebHostBuilder data, string configPath = "server.staticFiles")
        {
            return data.UseStaticFiles(Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<Vit.WebHost.StaticFilesConfig>(configPath));
        }


        /// <summary>
        /// 启用静态文件服务
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseStaticFiles(this IWebHostBuilder data, StaticFilesConfig config)
        {
            data?.Configure(app=>  app.UseStaticFiles(config));             

            return data;
        }
    }
}
