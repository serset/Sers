#region << 版本注释 - v1 >>
/*
 * ========================================================================
 * 版本：v1
 * 时间：190209
 * 作者：Lith   
 * Q  Q：755944120
 * 邮箱：litsoft@126.com
 * 
 * ========================================================================
*/
#endregion


using Microsoft.Extensions.Hosting;
using Sers.Apm.SkyWalking.Core.Model;
using System.Collections.Generic;

namespace Sers.Apm.SkyWalking
{
    public class SkyWalkingHost
    {
        public static Dictionary<string, string> config => Sers.Apm.SkyWalking.Core.SkyWalkingManage.config;

        public static  void Init()
        {

            var builder = new HostBuilder()
           //.ConfigureLogging(logging =>
           //{
           //    logging.AddConsole();
           //})
           .ConfigureServices((hostContext, services) =>
           {
               Sers.Apm.SkyWalking.Core.SkyWalkingManage.Init(services);
           });

            builder.RunConsoleAsync();
            //await builder.RunConsoleAsync();
            //Sers.Apm.SkyWalking.Core.SkyWalkingManage.Init(IocHelp.Instance.rootServiceCollection);    
        }


        public static void Publish(List<RequestModel> requests)
        {
            Sers.Apm.SkyWalking.Core.SkyWalkingManage.Send(requests);
        }
    }
}
