using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IWebHostBuilderExtensions_UseStaticFiles
    {
        /// <summary>
        /// 启用静态文件服务
        /// 
        /// <example>
        /// <code>
        ///  /* 映射静态文件。若不指定则不映射静态文件 */
        ///  "staticFiles": {
        /// 
        ///    /* 请求路径（可不指定）。demo："/file/static"。The relative request path that maps to static resources */
        ///    //"requestPath": "/file",
        /// 
        ///    /* 静态文件路径。可为相对路径或绝对路径。若为空或空字符串则为默认路径（wwwroot）。demo:"wwwroot/demo" */
        ///    //"rootPath": "wwwroot",
        /// 
        ///    /* 默认页面（可不指定）。An ordered list of file names to select by default. List length and ordering  may affect performance */
        ///    //"defaultFileNames": [],
        /// 
        ///    /* 是否可浏览目录(default false)。Enables directory browsing */
        ///    //"useDirectoryBrowser": false,
        /// 
        ///    /* 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"contentTypeMap.json"。若不指定（或指定的文件不存在）则不指定文件类型映射配置 */
        ///    "contentTypeMapFile": "contentTypeMap.json",
        /// 
        ///    /* 回应静态文件时额外添加的http回应头。可不指定。 */
        ///    "responseHeaders": {
        /// 
        ///      //设置浏览器静态文件缓存3600秒
        ///      "Cache-Control": "public,max-age=3600"
        ///    }
        ///  }
        ///        
        /// </code>
        /// </example>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="configPath">在appsettings.json文件中的路径。默认:"server.staticFiles"。</param>
        /// <returns></returns>
        public static IWebHostBuilder UseStaticFilesFromConfig(this IWebHostBuilder data, string configPath = "server.staticFiles")
        {
            return data.UseStaticFiles(Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<Vit.WebHost.StaticFilesConfig>(configPath));
        }


        /// <summary>
        /// 启用静态文件服务
        /// 
        /// <example>
        /// <code>
        ///  /* 映射静态文件。若不指定则不映射静态文件 */
        ///  "staticFiles": {
        /// 
        ///    /* 请求路径（可不指定）。demo："/file/static"。The relative request path that maps to static resources */
        ///    //"requestPath": "/file",
        /// 
        ///    /* 静态文件路径。可为相对路径或绝对路径。若为空或空字符串则为默认路径（wwwroot）。demo:"wwwroot/demo" */
        ///    //"rootPath": "wwwroot",
        /// 
        ///    /* 默认页面（可不指定）。An ordered list of file names to select by default. List length and ordering  may affect performance */
        ///    //"defaultFileNames": [],
        /// 
        ///    /* 是否可浏览目录(default false)。Enables directory browsing */
        ///    //"useDirectoryBrowser": false,
        /// 
        ///    /* 静态文件类型映射配置的文件路径。可为相对路径或绝对路径。例如"contentTypeMap.json"。若不指定（或指定的文件不存在）则不指定文件类型映射配置 */
        ///    "contentTypeMapFile": "contentTypeMap.json",
        /// 
        ///    /* 回应静态文件时额外添加的http回应头。可不指定。 */
        ///    "responseHeaders": {
        /// 
        ///      //设置浏览器静态文件缓存3600秒
        ///      "Cache-Control": "public,max-age=3600"
        ///    }
        ///  }
        ///        
        /// </code>
        /// </example>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseStaticFiles(this IWebHostBuilder data, StaticFilesConfig config)
        {
            data?.Configure(app=>  app.UseStaticFiles(config));             

            return data;
        }
    }
}
