using Sers.Core.Extensions;
using Sers.Core.Util.ConfigurationManager;
using Sers.ServiceStation.Module.Bearer;
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

            #region (x.x.1) 使用扩展消息队列
            //zmq
            Sers.ServiceCenter.ServiceCenter.Instance.mqMng.UseZmq();
            #endregion


            #region (x.x.2) 初始化 Apm.SkyWalking
            var skyWalking_Config = ConfigurationManager.Instance.GetByPath<Dictionary<string,string>>("Sers.Apm.SkyWalking");
            if (null != skyWalking_Config)
            {
                foreach (var item in skyWalking_Config)
                {
                    Sers.Apm.SkyWalking.SkyWalkingManage.config[item.Key] = item.Value;
                }
                //Sers.Apm.SkyWalking.SkyWalkingManage.config["SkyWalking:Transport:gRPC:Servers"] = "192.168.56.101:11800";
                Sers.Apm.SkyWalking.SkyWalkingManage.Init();
            }
            #endregion


            #region (x.x.3) 使用 Gover 服务治理 模块
            Sers.ServiceCenter.ServiceCenter.Instance.apiCenter.UseGover(Sers.ServiceCenter.ServiceCenter.Instance);
            #endregion


            #region (x.x.4) 使用Bearer            
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.Bearer.UseBearer"))
            {
                Sers.ServiceCenter.ServiceCenter.Instance.apiCenter.BeforeCallApi += BearerHelp.ConvertBearer;
            }
            #endregion

            //(x.x.5) 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
            Sers.ServiceCenter.ServiceCenter.Discovery();

            //(x.x.6)发现并注册系统Api
            Sers.ServiceCenter.ServiceCenter.Discovery(typeof(Sers.ServiceCenter.Controller.Controllers.ServiceStationController).Assembly);

            #endregion


            //(x.3) Start
            Sers.ServiceCenter.ServiceCenter.Start();


            //(x.4) RunAwait
            Sers.ServiceCenter.ServiceCenter.RunAwait();


        }
    }
}
