using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vit.Extensions
{
    public static partial class IServiceCollectionExtensions_ConfigureApp
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
        /// 添加app配置的同时不停用其他app配置。（调用IWebHostBuilder.Configure函数会停用掉之前的配置，仅最后一次生效）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="beforeConfig"></param>
        /// <param name="afterConfig"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureApp(this IServiceCollection services, Action<IApplicationBuilder> beforeConfig = null, Action<IApplicationBuilder> afterConfig = null)
        {
            return services?.AddTransient<IStartupFilter>(m =>
            {
                return new AutoRequestServicesStartupFilter { beforeConfig = beforeConfig, afterConfig = afterConfig };
            });
        }


         
        
    }
}
