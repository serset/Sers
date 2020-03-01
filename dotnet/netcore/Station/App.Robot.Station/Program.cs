using Sers.SersLoader;
using Sers.ServiceStation;

namespace App.Robot.Station
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //ServiceStation.AutoRun();

            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.0.1";


            //(x.1) Init
            ServiceStation.Init();

            //(x.2) Discovery
          
            ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly, new ApiLoaderConfig { apiStationName = "_robot_" });
            //ServiceStation.Instance.LoadApi();


            //(x.3) Start
            ServiceStation.Start();


            //(x.4) RunAwait
            ServiceStation.RunAwait();

        }
    }
}
