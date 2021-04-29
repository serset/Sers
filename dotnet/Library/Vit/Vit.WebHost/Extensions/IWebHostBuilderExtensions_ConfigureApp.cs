using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilderExtensions_ConfigureApp
    {      


        /// <summary>
        /// 添加app配置的同时不停用其他app配置。（调用IWebHostBuilder.Configure函数会停用掉之前的配置，仅最后一次生效）
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="beforeConfig"></param>
        /// <param name="afterConfig"></param>
        /// <returns></returns>
        public static IWebHostBuilder ConfigureApp(this IWebHostBuilder builder, Action<IApplicationBuilder> beforeConfig=null, Action<IApplicationBuilder> afterConfig=null)
        {
            return builder.ConfigureServices(services=> {
                services.AddTransient<IStartupFilter>(m=> {
                    return new IServiceCollectionExtensions_ConfigureApp.AutoRequestServicesStartupFilter { beforeConfig = beforeConfig , afterConfig = afterConfig };
                });
            });
        }

        
    }
}
