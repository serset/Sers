using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Api;
using System;
using System.Collections.Generic;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Mq.Mng;
using Sers.Core.Module.Api.Rpc;

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
            serviceStation.mqConnGuid= ServerMqMng.CurMqConnGuid;
            //注册api调用
            var serverMqMng = ServiceCenter.Instance.mqMng;
            serviceStation.OnSendRequest = (string connGuid, ApiMessage apiReqMessage) =>
            {
                if (serverMqMng.SendRequest(connGuid, apiReqMessage.Package(), out var replyOriData))
                {
                    return new List<ArraySegment<byte>>() { replyOriData };
                }          
                return new List<ArraySegment<byte>>();
            };
            var apiCenter = ServiceCenter.Instance.apiCenter; 
            apiCenter.ServiceStation_Regist(serviceStation);
            return new ApiReturn();            
        }      
    }
}
