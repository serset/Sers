using Sers.SersLoader;
using Sers.ServiceStation;

namespace Did.SersLoader.Demo
{
    public class Program
    {

        public static void Main(string[] args)
        {

            //ServiceStation.AutoRun();


            ////手动指定Station版本号
            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.1.1";

            //(x.1) Init
            ServiceStation.Init();


            #region (x.2)加载api
            //ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly);
            ServiceStation.Instance.LoadSsApi(typeof(Program).Assembly, new ApiLoaderConfig { apiStationName = "demo" });
            ServiceStation.Instance.LoadApi();
            #endregion





            //(x.3) Start
            ServiceStation.Start();


            //(x.4) RunAwait
            ServiceStation.RunAwait();

        }
    }
}
