using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Vit.Core.Util.Common;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IApplicationBuilderExtensions_UseStaticFiles
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
        public static IApplicationBuilder UseStaticFilesFromConfig(this IApplicationBuilder data, string configPath = "server.staticFiles")
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
        public static IApplicationBuilder UseStaticFiles(this IApplicationBuilder data, StaticFilesConfig config)
        {
            if (config == null || data == null) return data;


            #region (x.1)UseStaticFiles

            var staticfileOptions = new StaticFileOptions();

            //(x.x.1)requestPath
            if (!string.IsNullOrEmpty(config.requestPath))
            {
                staticfileOptions.RequestPath = new Microsoft.AspNetCore.Http.PathString(config.requestPath);
            }


            #region (x.x.2)FileProvider
            IFileProvider fileProvider=null;
            string rootPath;

            if (!string.IsNullOrWhiteSpace(rootPath=config.rootPath) && Directory.Exists(rootPath))
            {
                fileProvider = new PhysicalFileProvider(rootPath);
            }
            else if (Directory.Exists(rootPath = CommonHelp.GetAbsPath("wwwroot")))
            {
                fileProvider = new PhysicalFileProvider(rootPath);
            }
            else 
            {
                var dir = new DirectoryInfo("wwwroot");
                if(dir.Exists)
                    fileProvider = new PhysicalFileProvider(dir.FullName);
            }
            staticfileOptions.FileProvider = fileProvider;
            #endregion


            //(x.x.3)OnInitStaticFileOptions
            config.OnInitStaticFileOptions?.Invoke(staticfileOptions);

            #region (x.x.4)Response Headers
            if (config.responseHeaders != null)
            {
                staticfileOptions.OnPrepareResponse +=
                ctx =>
                {
                    var Headers = ctx.Context.Response.Headers;
                    foreach (var kv in config.responseHeaders)
                    {
                        Headers[kv.Key] = kv.Value;
                    }
                };
            }
            #endregion


            //(x.x.5)contentTypeProvider
            if (config.contentTypeProvider != null)
            {
                staticfileOptions.ContentTypeProvider = config.contentTypeProvider;
            }


            //(x.x.6)UseDefaultFiles
            if (config.defaultFileNames != null)
            {
                data.UseDefaultFiles(new DefaultFilesOptions
                {
                    DefaultFileNames = config.defaultFileNames,
                    FileProvider= fileProvider
                });
            }    


            //(x.x.7)UseStaticFiles
            data.UseStaticFiles(staticfileOptions);

            #endregion



            //(x.2)UseDirectoryBrowser
            if (config.useDirectoryBrowser == true)
            {
                var options = new DirectoryBrowserOptions {                   
                    RequestPath = staticfileOptions.RequestPath,
                    FileProvider = fileProvider
                };
                data.UseDirectoryBrowser(options);
            }


            return data;
        }
    }
}
