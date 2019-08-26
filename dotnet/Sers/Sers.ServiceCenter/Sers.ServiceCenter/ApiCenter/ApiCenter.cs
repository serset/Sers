using Sers.Core.Module.Api.Message;
using System;
using System.Collections.Generic;
using Sers.Core.Module.Rpc;
using Sers.Core.ServiceCenter.ApiTrace;
using Newtonsoft.Json;

namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public class ApiCenter
    {

        IApiCenterManage apiCenterManage = new ApiCenterManage_Simple();



        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        [JsonIgnore]
        public Action<IRpcContextData, ApiMessage> BeforeCallApi { get => apiCenterManage.BeforeCallApi; set => apiCenterManage.BeforeCallApi = value; }


        public void SetApiCenterManage(IApiCenterManage apiCenterManage)
        {
            this.apiCenterManage = apiCenterManage;
        }
     

        public List<ArraySegment<byte>> CallApi(ArraySegment<byte> oriData)
        {
            var requestMessage = new ApiMessage(oriData);

            var rpcData = RpcFactory.Instance.CreateRpcContextData().UnpackOriData(requestMessage.rpcContextData_OriData);      

            using (var trace = new ApiTrace(rpcData))
            {
                return apiCenterManage.CallApi(rpcData, requestMessage); 
            }
        }





        public void ServiceStation_Regist(ServiceStation serviceStation)
        {
            apiCenterManage.ServiceStation_Regist(serviceStation);
        }

        public void ServiceStation_Remove(string mqConnKey)
        {
            apiCenterManage.ServiceStation_Remove(mqConnKey);
        }


    }
}
