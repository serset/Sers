using Vit.Extensions;
using Sers.ServiceStation;
using static App.Ioc.Station.Controllers.DemoController;
using Vit.Ioc;

namespace App.Ioc.Station
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



            ServiceStation.Instance.localApiService.LoadSersApi(typeof(Program).Assembly); 

            ServiceStation.Start();


            ServiceStation.RunAwait();

        }
    }
}
