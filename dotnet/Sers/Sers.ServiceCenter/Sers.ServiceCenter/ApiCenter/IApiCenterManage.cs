using Newtonsoft.Json;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Rpc;
using System;
using System.Collections.Generic;


namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public interface IApiCenterManage
    {

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        [JsonIgnore]
        Action<IRpcContextData, ApiMessage> BeforeCallApi { get; set; }


        List<ArraySegment<byte>> CallApi(IRpcContextData rpcData, ApiMessage requestMessage);



        void ServiceStation_Regist(ServiceStation serviceStation);

        void ServiceStation_Remove(string connGuid);

    }
}
