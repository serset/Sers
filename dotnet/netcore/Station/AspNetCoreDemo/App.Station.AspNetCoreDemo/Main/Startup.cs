using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.FileProviders;

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

            services.AddMvc();
            ////注册Swagger生成器，定义一个和多个Swagger 文档
            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new Info { Title = "SwaggerDoc", Version = "v1" });

            //    // 设置SWAGER JSON和UI的注释路径。
            //    try
            //    {
            //        foreach (FileInfo fi in new DirectoryInfo(AppContext.BaseDirectory).GetFiles("*.xml"))
            //        {
            //            options.IncludeXmlComments(fi.FullName);
            //        }
            //    }
            //    catch { }

            //});

            //允许跨域访问   
            services.AddCors();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ////启用中间件服务生成Swagger作为JSON终结点
            //app.UseSwagger();
            ////启用中间件服务对swagger-ui，指定Swagger JSON终结点
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});


            //映射wwwroot
            if (env.IsDevelopment())
            {
                app.UseStaticFiles();
            }
            else
            {
                var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
                var staticfile = new StaticFileOptions();
                staticfile.FileProvider = new PhysicalFileProvider(wwwrootPath);
                app.UseStaticFiles(staticfile);
            }
           

            //允许跨域访问
            app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());



            app.UseMvc();
        }
    }
}
