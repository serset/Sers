using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
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


            //(x.x.2)FileProvider
            if (!string.IsNullOrWhiteSpace(config.rootPath))
            {
                staticfileOptions.FileProvider = new PhysicalFileProvider(config.rootPath);
            }
         

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
                    DefaultFileNames = config.defaultFileNames
                });
            }    


            //(x.x.7)UseStaticFiles
            data.UseStaticFiles(staticfileOptions);

            #endregion



            //(x.2)UseDirectoryBrowser
            if (config.useDirectoryBrowser == true)
            {
                data.UseDirectoryBrowser(new DirectoryBrowserOptions { FileProvider = staticfileOptions.FileProvider });
            }


            return data;
        }
    }
}
