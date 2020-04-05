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

            //(x.1) Init
            Sers.ServiceCenter.ServiceCenter.Init();


            #region (x.2) 初始化扩展模块

            #region (x.x.1)使用 Gover 服务治理 模块
            Sers.ServiceCenter.ServiceCenter.Instance.UseGover();
            #endregion


            #region (x.x.2)加载 ServiceCenter ApiEvent BeforeCallApi
            var BeforeCallApi = Sers.Core.Module.Api.ApiEvent.EventBuilder.LoadEvent_BeforeCallApi(ConfigurationManager.Instance.GetByPath<JArray>("Sers.ServiceCenter.BeforeCallApi"));
            if (BeforeCallApi != null) Sers.ServiceCenter.ServiceCenter.Instance.apiCenterService.BeforeCallApi += BeforeCallApi;
            #endregion

            //(x.x.3)从配置文件(appsettings.json::Sers.LocalApiService.ApiLoaders ) 加载api加载器并加载api        
            Sers.ServiceCenter.ServiceCenter.Instance.LoadApi();

            //(x.x.4)加载系统Api
            Sers.ServiceCenter.ServiceCenter.Instance.LoadSsApi(typeof(Sers.ServiceCenter.Controllers.ServiceStationController).Assembly);

            #endregion


            //(x.3) Start ServiceCenter
            if (!Sers.ServiceCenter.ServiceCenter.Start()) return;


            #region (x.4) Start gateway if needed
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


            //(x.5) RunAwait
            Sers.ServiceCenter.ServiceCenter.RunAwait();

        }
    }
}
