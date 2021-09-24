using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Vit.Extensions
{
    public static partial class IServiceCollectionExtensions_AllowAnyOrigin
    {

        /// <summary>
        /// 允许跨域访问。（尽早调用）
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AllowAnyOrigin(this IServiceCollection services )
        {
            AllowAnyOrigin_ConfigureServices(services);

            services.ConfigureApp(AllowAnyOrigin_Configure);

            return services;
        }


        public static void AllowAnyOrigin_ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
        }



        public static void AllowAnyOrigin_Configure(IApplicationBuilder app)
        {
            //支持 net core 5.0及以上版本
            //https://blog.csdn.net/Jack_Law/article/details/105212759

            app.UseCors(builder => builder
                //.AllowAnyOrigin()
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        }

    }
}
