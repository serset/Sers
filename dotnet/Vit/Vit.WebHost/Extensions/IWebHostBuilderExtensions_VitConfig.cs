using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Vit.Extensions
{
    public static class IWebHostBuilderExtensions_VitConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseVitConfig(this IWebHostBuilder data)
        {
            data.ConfigureServices(delegate (Microsoft.AspNetCore.Hosting.WebHostBuilderContext context, Microsoft.Extensions.DependencyInjection.IServiceCollection services)
            {
                services.Configure(delegate (Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
                {                    
                    //不限制body的大小
                    options.Limits.MaxRequestBodySize = Vit.Core.Util.ConfigurationManager.ConfigurationManager
                    .Instance.GetByPath<long?>("Vit.Kestrel.MaxRequestBodySize");
                });

                //解决Multipart body length limit 134217728 exceeded
                //services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
                //{
                //    x.ValueLengthLimit = int.MaxValue;
                //    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
                //});
            });
            return data;
        }
    }
}
