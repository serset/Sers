using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IWebHostBuilderExtensions_UseStaticFiles
    {
 
        public static IWebHostBuilder UseStaticFiles(this IWebHostBuilder data, StaticFilesConfig config)
        {
            data?.Configure(app=>  app.UseStaticFiles(config));             

            return data;
        }
    }
}
