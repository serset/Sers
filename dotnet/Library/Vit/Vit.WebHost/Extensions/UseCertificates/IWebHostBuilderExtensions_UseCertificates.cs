using Microsoft.AspNetCore.Hosting;

using Vit.WebHost.Extensions.UseCertificates;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilderExtensions_UseCertificates
    {
        /// <summary>
        /// 加载https证书 
        /// <example>
        /// <code>
        ///   //appsettings.json
        ///   //...
        ///   "server": {
        ///    /* ssl证书，可不指定。若urls中指定了https协议，请在此指定对应的https证书 */
        ///    "certificates": [
        ///      {
        ///        "filePath": "Data/ssl.pfx",
        ///        "password": "password"
        ///      }
        ///    ]
        ///   },
        ///   //...        
        /// </code>
        /// </example>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="configPath">在appsettings.json文件中的路径。默认:"server.certificates"。</param>
        /// <returns></returns>
        public static IWebHostBuilder UseCertificates(this IWebHostBuilder data, string configPath = "server.certificates")
        {
            data?.ConfigureServices(services=> services.UseCertificates(configPath)); 
            return data;
        }


        /// <summary>
        /// 加载https证书 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="certificates">证书配置</param>
        /// <returns></returns>
        public static IWebHostBuilder UseCertificates(this IWebHostBuilder data, CertificateInfo[] certificates)
        {
            data?.ConfigureServices(services => services.UseCertificates(certificates));
            return data;
        }
    }
}
