using Microsoft.AspNetCore.Hosting;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilderExtensions_UseUrlsFromConfig
    {
        /// <summary>
        /// Specify the urls the web host will listen on.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="urlsPath">在appsettings.json文件中的路径。默认:"server.urls"。其指定的值必须为字符串数组。</param>
        /// <param name="certificatesPath">在appsettings.json文件中的路径。默认:"server.certificates"。</param>
        /// <returns></returns>
        public static IWebHostBuilder UseUrlsFromConfig(this IWebHostBuilder data, string urlsPath = "server.urls", string certificatesPath = "server.certificates")
        {
            if (data == null) return data;

            #region (x.1)urls         
            var urls = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string[]>(urlsPath);
            if (urls != null && urls.Length > 0)
            {
                data.UseUrls(urls);
            }
            #endregion


            #region (x.2)certificates
            if (!string.IsNullOrEmpty(certificatesPath))
            {
                data.UseCertificates(certificatesPath);
            }
            #endregion


            return data;
        }
    }
}
