using Sers.Core.Module.Api.Rpc;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Sers.Core.CL.CommunicationManage;
using Sers.ServiceCenter.Entity;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;

namespace Sers.ServiceCenter.Controllers
{
    [SsStationName("_sys_")]
    public class ServiceStationController : IApiController
    {
        /// <summary>
        /// 注册站点。在站点初始化时调用
        /// </summary>
        /// <param name="serviceStation"></param>
        /// <returns></returns>
        [SsRoute("serviceStation/regist")]
        [SsName("注册站点")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Regist(ServiceStation serviceStation)
        {  
            serviceStation.connection = CommunicationManageServer.CurConn;

            if(serviceStation.connection!=null)
                serviceStation.connection.connTag = serviceStation?.serviceStationInfo?.serviceStationName;

            //注册站点       
            ServiceCenter.Instance.apiCenterService.ServiceStation_Regist(serviceStation);
            return true;            
        }





        /// <summary>
        /// 更新服务站点设备硬件信息
        /// </summary>
        /// <param name="serviceStation"></param>
        /// <returns></returns>
        [SsRoute("serviceStation/updateStationInfo")]
        [SsName("更新服务站点设备硬件信息")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn UpdateStationInfo(ServiceStation serviceStation)
        {
            serviceStation.connection = CommunicationManageServer.CurConn;          
            return ServiceCenter.Instance.apiCenterService.ServiceStation_UpdateStationInfo(serviceStation); 
        }


    }
}
