using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Vit.Core.Util.Common;
using Vit.WebHost.Extensions.UseCertificates;

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
            var configs = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<CertificateInfo[]>(configPath);
            return data.UseCertificates(configs);
        }




        /// <summary>
        /// 加载https证书
        /// </summary>
        /// <param name="data"></param>
        /// <param name="certificates">证书配置</param>
        /// <returns></returns>
        public static IServiceCollection UseCertificates(this IServiceCollection data, CertificateInfo[] certificates)
        {
            if (certificates == null || certificates.Length == 0) return data;

            //var certificate = new X509Certificate2(@"L:\Code\sersit-com-iis-0923120142.pfx", "password");
            ////var dnsName = certificate.GetNameInfo(X509NameType.SimpleName, false);
            //var dnsName = certificate.GetNameInfo(X509NameType.DnsName, false);


            //(x.1)构建证书字典
            X509Certificate2 defaultCert = null;
            Dictionary<string, X509Certificate2> certMap = new Dictionary<string, X509Certificate2>();
            foreach (var config in certificates)
            {
                var certificate = new X509Certificate2(CommonHelp.GetAbsPath(config.filePath), config.password);
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
