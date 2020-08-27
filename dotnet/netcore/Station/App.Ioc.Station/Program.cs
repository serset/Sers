using Vit.Extensions;
using Sers.ServiceStation;
using static App.Ioc.Station.Controllers.DemoController;
using Vit.Ioc;
using Sers.Core.Module.Api.LocalApi.Event;

namespace App.Ioc.Station
{
    public class Program
    {
        public static void Main(string[] args)
        {

            ServiceStation.Init();


            #region ioc
            //use ioc
            LocalApiEventMng.Instance.UseIoc();

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
