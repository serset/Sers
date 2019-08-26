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


            #region Init

            //Sers.ServiceCenter.ServiceCenter.Instance.serviceStationInfo.stationVersion = "1.3";

            Sers.ServiceCenter.ServiceCenter.Init();

            //使用扩展消息队列            
            Sers.ServiceCenter.ServiceCenter.Instance.mqMng.UseZmq();


            #region 初始化 Apm.SkyWalking
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

            //使用 Gover 服务治理 模块
            Sers.ServiceCenter.ServiceCenter.Instance.apiCenter.UseGover(Sers.ServiceCenter.ServiceCenter.Instance);

            #region 使用Bearer            
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.Bearer.UseBearer"))
            {
                Sers.ServiceCenter.ServiceCenter.Instance.apiCenter.BeforeCallApi += BearerHelp.ConvertBearer;
            }
            #endregion


            //发现并注册系统Api
            Sers.ServiceCenter.ServiceCenter.Discovery(typeof(Sers.ServiceCenter.Controller.Controllers.ServiceStationController).Assembly);

            #endregion


            Sers.ServiceCenter.ServiceCenter.Start();


            Sers.ServiceCenter.ServiceCenter.RunAwait();
       


        }
    }
}
