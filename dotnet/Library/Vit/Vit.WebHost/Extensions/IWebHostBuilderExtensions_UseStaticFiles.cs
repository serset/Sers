using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IWebHostBuilderExtensions_UseStaticFiles
    {

        public static IWebHostBuilder UseStaticFilesFromConfig(this IWebHostBuilder data, string configPath = "server.staticFiles")
        {
            //配置静态文件
            return data.UseStaticFiles(Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<Vit.WebHost.StaticFilesConfig>(configPath));
        }


        public static IWebHostBuilder UseStaticFiles(this IWebHostBuilder data, StaticFilesConfig config)
        {
            data?.Configure(app=>  app.UseStaticFiles(config));             

            return data;
        }
    }
}
