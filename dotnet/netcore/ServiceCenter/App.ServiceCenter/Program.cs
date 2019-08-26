using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Util.ConfigurationManager;
using System.Collections.Generic;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //(x.1) Init
            Sers.ServiceCenter.ServiceCenter.Init();


            #region (x.2) 初始化扩展模块

            #region (x.x.1) 使用 Gover 服务治理 模块
            Sers.ServiceCenter.ServiceCenter.Instance.apiCenterService.UseGover(Sers.ServiceCenter.ServiceCenter.Instance);
            #endregion


            #region (x.x.2)构建 Api Event BeforeCallApi
            var BeforeCallApi = Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi.EventBuilder.LoadEvent(ConfigurationManager.Instance.GetByPath<JArray>("Sers.Api.BeforeCallApi"));
            if (BeforeCallApi != null) Sers.ServiceCenter.ServiceCenter.Instance.apiCenterService.BeforeCallApi += BeforeCallApi;
            #endregion

            //(x.x.3) 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
            Sers.ServiceCenter.ServiceCenter.Discovery();

            //(x.x.4)发现并注册系统Api
            Sers.ServiceCenter.ServiceCenter.Discovery(typeof(Sers.ServiceCenter.Controller.Controllers.ServiceStationController).Assembly);

            #endregion


            //(x.3) Start
            Sers.ServiceCenter.ServiceCenter.Start();


            //(x.4) RunAwait
            Sers.ServiceCenter.ServiceCenter.RunAwait();


        }
    }
}
