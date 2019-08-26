using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.Mq.MqManager;

namespace Sers.ServiceCenter.Controller.Controllers
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
            serviceStation.mqConn = ServerMqManager.CurMqConn;

            if(serviceStation.mqConn!=null)
                serviceStation.mqConn.connTag = serviceStation?.serviceStationInfo?.serviceStationName;

            //注册api调用           
            serviceStation.OnSendRequest = ServiceCenter.Instance.mqMng.Station_SendRequestAsync;
            var apiCenter = ServiceCenter.Instance.apiCenterService; 
            apiCenter.ServiceStation_Regist(serviceStation);
            return true;            
        }      
    }
}
