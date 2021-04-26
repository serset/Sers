using Microsoft.AspNetCore.Hosting;

namespace Vit.Extensions
{
    public static class IWebHostBuilderExtensions_UseUrlsFromConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="configPath">在appsettings.json文件中的路径。默认:"server.urls"。其指定的值必须为字符串数组。</param>
        /// <returns></returns>
        public static IWebHostBuilder UseUrlsFromConfig(this IWebHostBuilder data, string configPath = "server.urls")
        {
            if (data == null) return data;

            var urls = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string[]>("server.urls");

            if (urls != null && urls.Length > 0)
            {
                data.UseUrls(urls);
            }
            return data;
        }
    }
}
