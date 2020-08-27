using Sers.ServiceStation;

namespace Did.SersLoader.HelloWorld
{
    public class Program
    {

        public static void Main(string[] args)
        {

            //ServiceStation.AutoRun();


            //(x.1) Init
            ServiceStation.Init();


            //(x.2)加载api
            ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly);

            //(x.3) Start
            ServiceStation.Start();


            //(x.4) RunAwait
            ServiceStation.RunAwait();

        }
    }
}
