using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Vit.WebHost
{
    public class RunArg
    {


        public Func<IWebHostBuilder> OnCreateWebHostBuilder = () => new WebHostBuilder().UseKestrel();

        /// <summary>
        /// 是否异步运行.Runs a web application and returns a Task that only completes when the token is triggered or shutdown is triggered.
        /// </summary>
        public bool RunAsync = false;

        #region wwwrootPath       
        /// <summary>
        /// 静态文件路径。若不指定(null)则不映射静态文件
        /// </summary>
        private string _wwwrootPath = null;
        /// <summary>
        /// 静态文件路径。可为相对路径或绝对路径。若为空字符串则默认为当前目录下的wwwroot文件夹。若不指定(null)则不映射静态文件。
        /// </summary>
        public string wwwrootPath
        {
            get => _wwwrootPath;
            set
            {
                #region (x.1) get fullPath
                string fullPath = value;
                if ("" == fullPath)
                {
                    fullPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
                }
                else if (fullPath != null)
                {
                    if (!Directory.Exists(fullPath))
                    {
                        fullPath = Path.Combine(AppContext.BaseDirectory, fullPath);
                    }
                }

                if (string.IsNullOrEmpty(fullPath)) 
                {
                    fullPath = null;
                }
                else
                {
                    var dir = new DirectoryInfo(fullPath);
                    if (dir.Exists)
                    {
                        fullPath = dir.FullName;
                    }
                    else
                    {
                        fullPath = null;
                    }
                }
                #endregion

                _wwwrootPath = fullPath;
            }
        }
        #endregion


        /// <summary>
        /// 初始化 wwwroot静态文件配置的操作
        /// </summary>
        public Action<StaticFileOptions> OnInitStaticFileOptions;

        public Action<IServiceCollection> OnConfigureServices = null;
        public Action<IApplicationBuilder> OnConfigure = null;
        public string[] urls;

        /// <summary>
        /// 允许跨域访问
        /// </summary>
        public bool allowAnyOrigin = false;


        public RunArg SetPort(int port = 8888)
        {
            urls = new string[] { "http://*:" + port };
            return this;
        }
    }
}
