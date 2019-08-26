using Sers.Core.Extensions;
using Sers.Core.Util.Ioc;
using Sers.ServiceStation;
using static App.Ioc.Station.Controllers.Demo.DemoController;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {

            ServiceStation.Init();


            #region ioc
            //use ioc
            Sers.Core.Module.Rpc.RpcFactory.Instance.UseIoc();

            IocHelp.AddSingleton<ISingleton, ArgModel>();
            IocHelp.AddScoped<IScoped, ArgModel>();
            IocHelp.AddTransient<ITransient, ArgModel>(); 

            IocHelp.Update();
            #endregion




            ServiceStation.Discovery(typeof(Program).Assembly); 

            ServiceStation.Start();


            ServiceStation.RunAwait();

        }
    }
}
