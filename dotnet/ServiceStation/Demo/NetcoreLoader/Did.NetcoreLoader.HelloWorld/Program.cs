using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.LocalApi.Event;
using Sers.ServiceStation;
using Vit.Extensions;

namespace Did.NetcoreLoader.HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (null != Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Sers"))
            {
                //start by sers

                ServiceStation.Init();

                LocalApiEventMng.Instance.UseIoc();

                #region (x.x)加载api             
                ServiceStation.Instance.localApiService.LoadNetcoreApi(typeof(Program).Assembly);
                ServiceStation.Instance.LoadApi(); 
                #endregion


                ServiceStation.Start();

                ServiceStation.RunAwait();

            }
            else
            {
                //start by AspNetCore Original
                CreateWebHostBuilder(args).Build().Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               .UseUrls(Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string[]>("server.urls"))
               .UseStartup<Startup>();
    }
}
