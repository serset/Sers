
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.LocalApi;
using Sers.Serslot.Mode.Async;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilder_UseSerslot_Extensions
    {
        public static IWebHostBuilder UseSerslot(this IWebHostBuilder hostBuilder)
        {
            if (null == Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.Get<JToken>("Sers"))
            {
                return hostBuilder;
            }



            var server = new SerslotServer();
            server.InitPairingToken(hostBuilder);

            LocalApiServiceFactory.CreateLocalApiService = () => new Sers.Serslot.Mode.Async.LocalApiService(server);


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
