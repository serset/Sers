
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sers.Serslot21;

namespace Vit.Extensions
{
    public static partial class WebHostBuilderSerslotExtensions
    {
        public static IWebHostBuilder UseSerslot(this IWebHostBuilder hostBuilder)
        {
            if (null == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Sers"))
            {
                return hostBuilder;
            }


            var server = new SerslotServer();
            server.InitPairingToken(hostBuilder);
       
            return hostBuilder.ConfigureServices(services =>
            {                 
                services.AddSingleton<IServer>((serviceProvider)=> {
                    server.serviceProvider = serviceProvider;
                    return server;
                });
            });


        }



    }
}
