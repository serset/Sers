using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json.Linq;
using Sers.Serslot;
using Microsoft.Extensions.DependencyInjection;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilder_UseSerslot_Extensions
    {
        /// <summary>
        /// 尝试使用Serslot。若未指定配置则不启用Serslot(即appsettings.json未指定Sers节点时不做任何操作)。
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IWebHostBuilder TryUseSerslot(this IWebHostBuilder hostBuilder)
        {
            if (null == Vit.Core.Util.ConfigurationManager.Appsettings.json.Get<JToken>("Sers"))
            {
                return hostBuilder;
            }

            return hostBuilder.UseSerslot();
        }

        /// <summary>
        /// 使用Serslot。若未指定配置则使用默认配置强制启用Serslot。
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseSerslot(this IWebHostBuilder hostBuilder)
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
