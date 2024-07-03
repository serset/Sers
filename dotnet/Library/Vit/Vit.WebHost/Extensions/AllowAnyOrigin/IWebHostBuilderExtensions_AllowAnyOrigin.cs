using Microsoft.AspNetCore.Hosting;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilderExtensions_AllowAnyOrigin
    {

        /// <summary>
        /// 允许跨域访问。（尽早调用）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IWebHostBuilder AllowAnyOrigin(this IWebHostBuilder data)
        {
            return data.ConfigureServices(services => services.AllowAnyOrigin());
        }


    }
}
