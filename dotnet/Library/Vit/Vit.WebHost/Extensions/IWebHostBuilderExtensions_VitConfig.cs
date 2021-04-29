using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilderExtensions_VitConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseVitConfig(this IWebHostBuilder data)
        {
            data.ConfigureServices(delegate (Microsoft.AspNetCore.Hosting.WebHostBuilderContext context, Microsoft.Extensions.DependencyInjection.IServiceCollection services)
            {
                services.Configure(delegate (Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
                {                    
                    //限制body的大小
                    options.Limits.MaxRequestBodySize = Vit.Core.Util.ConfigurationManager.ConfigurationManager
                    .Instance.GetByPath<long?>("Vit.Kestrel.MaxRequestBodySize");
                });

                //解决Multipart body length limit 134217728 exceeded
                services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
                {
                    
                    var ValueLengthLimit = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int?>("Vit.Kestrel.ValueLengthLimit");
                    if (ValueLengthLimit.HasValue)
                    {
                        x.ValueLengthLimit = ValueLengthLimit.Value;
                    }

                    var MultipartBodyLengthLimit = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<long?>("Vit.Kestrel.MultipartBodyLengthLimit");
                    if (MultipartBodyLengthLimit.HasValue)
                    {
                        x.MultipartBodyLengthLimit = MultipartBodyLengthLimit.Value;// In case of multipart
                    }        
                });
            });

            if (null == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Logging"))
            {
                data.ConfigureLogging((Microsoft.Extensions.Logging.ILoggingBuilder logging) =>
                {
                    logging.ClearProviders();
                });
            }         

            return data;
        }
    }
}
