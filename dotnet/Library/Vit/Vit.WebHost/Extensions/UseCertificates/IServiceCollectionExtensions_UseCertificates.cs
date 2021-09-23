using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Vit.Core.Util.Common;

namespace Vit.Extensions
{
    public static partial class IServiceCollectionExtensions_UseCertificates
    {
        /// <summary>
        /// 加载https证书 
        /// <example>
        /// <code>
        ///   //appsettings.json
        ///   //...
        ///   "server": {
        ///    /* https证书配置，可不指定。若urls中指定了https协议，请在此指定对应的https证书 */
        ///    "certificates": [
        ///      {
        ///        "filePath": "data/serset-com-iis-0923120142.pfx",
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
        public static IServiceCollection UseCertificates(this IServiceCollection data, string configPath = "server.certificates")
        {

            //var certificate = new X509Certificate2(@"L:\Code\AliSvn\Lith\ssl证书\sersit-com-iis-0923120142.pfx", "Admin0123");
            ////var dnsName = certificate.GetNameInfo(X509NameType.SimpleName, false);
            //var dnsName = certificate.GetNameInfo(X509NameType.DnsName, false);


            //(x.1)构建证书字典
            X509Certificate2 defaultCert = null;
            Dictionary<string, X509Certificate2> certMap = new Dictionary<string, X509Certificate2>();
            foreach (var item in Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<Newtonsoft.Json.Linq.JArray>(configPath)
                ?? new Newtonsoft.Json.Linq.JArray())
            {
                var certificate = new X509Certificate2(CommonHelp.GetAbsPath(item["filePath"].ToString()), item["password"].ToString());
                var dnsName = certificate.GetNameInfo(X509NameType.DnsName, false);
                certMap[dnsName] = certificate;

                defaultCert = certificate;
            }

            if (defaultCert != null)
            {
                data?.Configure((Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options) =>
                {
                    options.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.ServerCertificateSelector = (context, name) =>
                            {
                                if (name != null && certMap.TryGetValue(name, out var cert))
                                {
                                    return cert;
                                }
                                return defaultCert;
                            };
                    });
                });
            }

            return data;
        }


    }
}
