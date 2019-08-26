using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Sers.Core.Extensions;
using Sers.Core.Util.ConfigurationManager;
using Sers.ServiceStation;

namespace WebTest
{
    public class Program
    {
 
        public static void Main(string[] args)
        {
            //*/

            //start by sers
         
            ServiceStation.Init(); 

            ServiceStation.Instance.localApiService.UseAspNetCoreApiDiscovery();

            ServiceStation.Discovery(typeof(Program).Assembly);

            ServiceStation.Start();

            ServiceStation.RunAwait();

            /*/
             
            //start by AspNetCore Original
            BuildWebHost(args).Run();

            //*/
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .UseUrls(ConfigurationManager.Instance.GetByPath<string[]>("Sers.WebHost.urls"))
              .UseStartup<Startup>()
                .Build();
    }
}
