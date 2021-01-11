using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Vit.Core.Util.Common;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IApplicationBuilderExtensions_UseStaticFiles
    {
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
