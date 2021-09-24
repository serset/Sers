using Microsoft.AspNetCore.Builder;
using Vit.WebHost.Extensions.HttpsRedirection;

namespace Vit.Extensions
{
    public static partial class IApplicationBuilderExtensions_UseStaticFiles
    {
        /// <summary>
        /// 重定向所有的http请求到https
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpsRedirection(this IApplicationBuilder data, HttpsRedirectionConfig config)
        {
            return data.UseMiddleware<HttpsRedirectionMiddleware>(config);
        }


        /// <summary>
        /// 重定向所有的http请求到https
        /// </summary>
        /// <param name="data"></param>
        /// <param name="host">重定向的地址。若不指定，则使用发起请求的host</param>
        /// <param name="port">重定向的端口号。若不指定，则使用发起请求的port</param>
        /// <param name="statusCode">The status code used for the redirect response. The default is 307.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpsRedirection(this IApplicationBuilder data, string host = null, int? port = null, int statusCode = 307)
        {
            return data.UseHttpsRedirection(new HttpsRedirectionConfig { host = host, port = port, statusCode = statusCode });
        }
    }
}
