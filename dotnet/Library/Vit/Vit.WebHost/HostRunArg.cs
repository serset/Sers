using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Vit.WebHost.Extensions.HttpsRedirection;
using Vit.WebHost.Extensions.UseCertificates;

namespace Vit.WebHost
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class HostRunArg
    {

        public Func<IWebHostBuilder> OnCreateWebHostBuilder = () => new WebHostBuilder().UseKestrel();

        /// <summary>
        /// 
        /// </summary>       
        [JsonProperty]
        public string[] urls;


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty]
        public CertificateInfo[] certificates { get; set; }


        /// <summary>
        /// 
        /// </summary>       
        [JsonProperty]
        public HttpsRedirectionConfig httpsRedirection { get; set; }


        /// <summary>
        /// 是否允许跨域访问，默认true
        /// </summary>       
        [JsonProperty]
        public bool allowAnyOrigin = true;


        /// <summary>
        /// 映射静态文件，可不指定
        /// </summary>       
        [JsonProperty]
        public StaticFilesConfig staticFiles;


        /// <summary>
        /// 是否异步运行.Runs a web application and returns a Task that only completes when the token is triggered or shutdown is triggered.
        /// </summary>      
        public bool RunAsync = false;

        public Action<IServiceCollection> OnConfigureServices = null;
        public Action<IApplicationBuilder> BeforeConfigure = null;
        public Action<IApplicationBuilder> OnConfigure = null;



        public HostRunArg SetPort(int port = 8888)
        {
            urls = new string[] { "http://*:" + port };
            return this;
        }

    }
}
