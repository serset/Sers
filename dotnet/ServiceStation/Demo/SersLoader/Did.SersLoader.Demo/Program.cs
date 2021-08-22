using Sers.SersLoader;
using Sers.ServiceStation;

using System.Collections.Concurrent;

namespace Did.SersLoader.Demo
{
    public class Program
    {

        public static void Main(string[] args)
        {


            BlockingCollection<object> queue=new BlockingCollection<object>(10);
            //queue.BoundedCapacity
            try
            {
                for(var t=0;t<10;t++)
                queue.Add(new object());

                var td = queue.TryAdd(new object());

                queue.Add(new object());


            }
            catch (System.Exception ex)
            {

                throw;
            }


            //ServiceStation.AutoRun();


            ////手动指定Station版本号
            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.1.1";

            //(x.1) Init
            ServiceStation.Init();


            #region (x.2)加载api
            //ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly);
            ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly, new ApiLoaderConfig { apiStationName = "demo" });
            ServiceStation.Instance.LoadApi();
            #endregion





            //(x.3) Start
            ServiceStation.Start();


            //(x.4) RunAwait
            ServiceStation.RunAwait();

        }
    }
}
