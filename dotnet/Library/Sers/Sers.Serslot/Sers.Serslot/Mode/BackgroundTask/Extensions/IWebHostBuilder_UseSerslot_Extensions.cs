
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Sers.Serslot.Mode.BackgroundTask;

namespace Vit.Extensions
{
    static partial class IWebHostBuilder_UseSerslot_Extensions
    {
        internal static IWebHostBuilder UseSerslot_BackgroundTask(this IWebHostBuilder hostBuilder)
        {
            var server = new SerslotServer();
            server.InitPairingToken(hostBuilder);

            return hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IServer>((serviceProvider) =>
                {
                    server.serviceProvider = serviceProvider;
                    return server;
                });
            });

        }

    }
}
