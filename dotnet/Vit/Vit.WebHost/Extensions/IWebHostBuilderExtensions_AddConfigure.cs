using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilderExtensions_AddConfigure
    {
        public class AutoRequestServicesStartupFilter : IStartupFilter
        {
            public Action<IApplicationBuilder> beforeConfig;
            public Action<IApplicationBuilder> afterConfig;
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    beforeConfig?.Invoke(builder);              
                    next(builder);
                    afterConfig?.Invoke(builder);
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="beforeConfig"></param>
        /// <param name="afterConfig"></param>
        /// <returns></returns>
        public static IWebHostBuilder AddConfigure(this IWebHostBuilder builder, Action<IApplicationBuilder> beforeConfig=null, Action<IApplicationBuilder> afterConfig=null)
        {
            return builder.ConfigureServices(services=> {
                services.AddTransient<IStartupFilter>(m=> {
                    return new AutoRequestServicesStartupFilter { beforeConfig = beforeConfig , afterConfig = afterConfig };
                });
            });
        }

        
    }
}
