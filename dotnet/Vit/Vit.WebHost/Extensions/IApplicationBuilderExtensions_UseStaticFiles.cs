﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using Vit.WebHost;

namespace Vit.Extensions
{
    public static class IApplicationBuilderExtensions_UseStaticFiles
    { 
        public static IApplicationBuilder UseStaticFiles(this IApplicationBuilder data, StaticFilesConfig config)
        {
            if (config == null || data==null) return data;         


            //(x.1)
            var staticfileOptions = new StaticFileOptions();

            #region (x.2)FileProvider
            if (!string.IsNullOrWhiteSpace(config.rootPath))
            {
                staticfileOptions.FileProvider = new PhysicalFileProvider(config.rootPath);
            }
            #endregion

            //(x.3)OnInitStaticFileOptions
            config.OnInitStaticFileOptions?.Invoke(staticfileOptions);

            #region (x.4)Response Headers
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


            //(x.5)contentTypeProvider
            if (config.contentTypeProvider != null) 
            {
                staticfileOptions.ContentTypeProvider = config.contentTypeProvider;
            }



            #region (x.6)UseDefaultFiles
            if (config.defaultFileNames != null) 
            {
                data.UseDefaultFiles(new DefaultFilesOptions
                {
                    DefaultFileNames = config.defaultFileNames
                });
            }
            #endregion


            //(x.7)UseStaticFiles
            data.UseStaticFiles(staticfileOptions);


            //(x.8)UseDirectoryBrowser
            if (config.useDirectoryBrowser == true) 
            {               
                data.UseDirectoryBrowser(new DirectoryBrowserOptions { FileProvider = staticfileOptions.FileProvider });
            }


            return data;
        }
    }
}