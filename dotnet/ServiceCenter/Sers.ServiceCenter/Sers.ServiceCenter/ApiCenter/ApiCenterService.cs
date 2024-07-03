using System;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.ServiceCenter.Entity;

namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public abstract class ApiCenterService
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(IOrganizeConnection conn, Object sender, ArraySegment<byte> apiRequest, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        {
            CommunicationManageServer.CurConn = conn;

            CallApiAsync(new ApiMessage(apiRequest), sender, callback);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(IOrganizeConnection conn, Object sender, ApiMessage apiRequestMessage, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        {
            CommunicationManageServer.CurConn = conn;

            CallApiAsync(apiRequestMessage, sender, callback);
        }


        #region interface

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        [JsonIgnore]
        public Action<RpcContextData, ApiMessage> BeforeCallApi;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CallApiAsync(ApiMessage requestMessage, Object sender, Action<object, Vit.Core.Util.Pipelines.ByteData> callback);



        public abstract void ServiceStation_Regist(ServiceStation serviceStation);


        /// <summary>
        /// 更新服务站点设备硬件信息
        /// </summary>
        /// <param name="serviceStation"></param>
        public abstract bool ServiceStation_UpdateStationInfo(ServiceStation serviceStation);

        public abstract void ServiceStation_Remove(IOrganizeConnection conn);
        #endregion



    }
}
