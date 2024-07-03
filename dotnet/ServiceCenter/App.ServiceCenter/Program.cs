using Newtonsoft.Json.Linq;

using Sers.Gateway;

using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;

namespace App.ServiceCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // #1 Init
            Sers.ServiceCenter.ServiceCenter.Init();


            #region #2 init extension modules

            #region  ##1 use Gover service manage module
            Sers.ServiceCenter.ServiceCenter.Instance.UseGover();
            #endregion


            #region ##2 load ServiceCenter ApiEvent BeforeCallApi
            var BeforeCallApi = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_BeforeCallApi(Appsettings.json.GetByPath<JArray>("Sers.ServiceCenter.BeforeCallApi"));
            if (BeforeCallApi != null) Sers.ServiceCenter.ServiceCenter.Instance.apiCenterService.BeforeCallApi += BeforeCallApi;
            #endregion

            // ##3 Load ApiLoadera then load apis from configuration (appsettings.json::Sers.LocalApiService.ApiLoaders )
            Sers.ServiceCenter.ServiceCenter.Instance.LoadApi();

            // ##4 Load system manage apis
            Sers.ServiceCenter.ServiceCenter.Instance.LoadSsApi(typeof(Sers.ServiceCenter.Controllers.ServiceStationController).Assembly);

            #endregion


            // #3 Start ServiceCenter
            if (!Sers.ServiceCenter.ServiceCenter.Start()) return;


            #region #4 Start gateway if needed
            try
            {
                GatewayHelp.Bridge();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
                return;
            }
            #endregion


            // #5 RunAwait
            Sers.ServiceCenter.ServiceCenter.RunAwait();

        }
    }
}
