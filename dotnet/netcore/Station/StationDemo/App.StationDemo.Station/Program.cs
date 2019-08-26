using App.StationDemo.Station.Controllers.PubSub;
using Sers.Core.Extensions;
using Sers.Core.Module.SersDiscovery;
using Sers.ServiceStation;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //手动指定Station版本号
            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "1.3";


            //(x.1) Init
            ServiceStation.Init();


            //(x.x)使用扩展消息队列
            //ServiceStation.Instance.mqMng.UseZmq();

            //(x.2) Discovery
            //ServiceStation.Discovery(new DiscoveryConfig { assembly= typeof(Program).Assembly,routePrefix_Force="v3",apiStationName_Force= "StationDemo" });
            //ServiceStation.Discovery(typeof(Program).Assembly);
            ServiceStation.Discovery();


            //(x.3) Start
            ServiceStation.Start();

            //(x.x) Subscribe
            if (ServiceStation.IsRunning)
            {
                SubscriberDemo.Subscribe();
            }

            //(x.4) RunAwait
            ServiceStation.RunAwait();

        }
    }
}
