using Sers.SersLoader;
using Sers.ServiceStation;

using Vit.Extensions;

namespace App.Robot.Station
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //ServiceStation.AutoRun();

            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.0.1";


            // #1 Init
            ServiceStation.Init();

            // #2 Load apis
            ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly, new ApiLoaderConfig { apiStationName = "_robot_" });
            ServiceStation.Instance.localApiService.LoadApi_StaticFiles();


            // #3 Start
            ServiceStation.Start();


            // #4 RunAwait
            ServiceStation.RunAwait();

        }
    }
}
