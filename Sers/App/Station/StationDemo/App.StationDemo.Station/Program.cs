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
            //ServiceStation.Instance.serviceStationInfo.stationVersion = "1.3";

            ServiceStation.Init();

            #region 使用扩展消息队列            
            //ServiceStation.Instance.mqMng.UseZmq();
            #endregion

            ServiceStation.Discovery(typeof(Program).Assembly);
            // ServiceStation.Discovery(new DiscoveryConfig { assembly= typeof(Program).Assembly,routePrefix_Force="v3",apiStationName_Force= "StationDemo" });

            ServiceStation.Start();

            if (ServiceStation.IsRunning)
            {
                SubscriberDemo.Subscribe();
            }


            ServiceStation.RunAwait();

        }
    }
}
