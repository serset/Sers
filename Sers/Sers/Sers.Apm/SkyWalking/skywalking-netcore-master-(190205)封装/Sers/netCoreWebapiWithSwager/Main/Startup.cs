using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Sers.Apm.SkyWalking.Core;

namespace WebTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            SkyWalkingManage.config["SkyWalking:Transport:gRPC:Servers"] = "192.168.56.101:11800";

            SkyWalkingManage.Init(services);
            //services.AddSersSkyWalking();

           //        services.AddSkyWalking(option =>
           // {
           //     option.ApplicationCode = "lith1";
           //     option.DirectServers = "192.168.56.101:11800";
           //     // 每三秒采样的Trace数量,-1 为全部采集
           //     option.SamplePer3Secs = -1;
           // }).AddEntityFrameworkCore(c => { c.AddPomeloMysql(); })
           //.AddHttpClient();


            services.AddMvc();
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "SwaggerDoc", Version = "v1" });

                // 设置SWAGER JSON和UI的注释路径。
                try
                {
                    foreach (FileInfo fi in new DirectoryInfo(AppContext.BaseDirectory).GetFiles("*.xml"))
                    {
                        options.IncludeXmlComments(fi.FullName);
                    }
                }
                catch { }

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
             

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
