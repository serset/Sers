using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.App;
using Sers.ServiceStation;

namespace Main
{
    public class Program
    {

        public static void Main(string[] args)
        {

            ServiceStation.AutoRun();


            ////手动指定Station版本号
            ////Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.0.1";


            ////(x.1) Init
            //ServiceStation.Init();

            ////(x.2) Discovery
            ////ServiceStation.Discovery(new DiscoveryConfig { assembly= typeof(Program).Assembly,routePrefix_Force="v3",apiStationName_Force= "StationDemo" });
            ////ServiceStation.Discovery(typeof(Program).Assembly);
            //ServiceStation.Discovery();


            ////(x.3) Start
            //ServiceStation.Start();


            ////(x.4) RunAwait
            //ServiceStation.RunAwait();

        }
    }
}
