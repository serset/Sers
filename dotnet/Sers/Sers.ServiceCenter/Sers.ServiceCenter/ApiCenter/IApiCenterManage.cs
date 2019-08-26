using Newtonsoft.Json;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Mq.Mq;
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

        void CallApiAsync(IRpcContextData rpcData, ApiMessage requestMessage, Object sender, Action<object, List<ArraySegment<byte>>> callback); 



        void ServiceStation_Regist(ServiceStation serviceStation);

        void ServiceStation_Remove(IMqConn mqConn);

    }
}
