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
          
            ServiceStation.Instance.LoadSsApi(typeof(Program).Assembly, new Sers.Core.Module.ApiLoader.ApiLoaderConfig { apiStationName = "_robot_" });
            //ServiceStation.Instance.LoadApi();


            //(x.3) Start
            ServiceStation.Start();


            //(x.4) RunAwait
            ServiceStation.RunAwait();

        }
    }
}
