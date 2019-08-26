using Newtonsoft.Json.Linq;
using Sers.Core.Module.App;
using Sers.ServiceStation;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {

            #region mcData
            SersApplication.serviceStationInfo.info = new JObject { ["mcData"] = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<JArray>("Mc_Computer.mcData") };
            #endregion



            ServiceStation.Init();

            ServiceStation.Discovery(typeof(Program).Assembly);

            ServiceStation.Start();

            ServiceStation.RunAwait();

        }
    }
}
