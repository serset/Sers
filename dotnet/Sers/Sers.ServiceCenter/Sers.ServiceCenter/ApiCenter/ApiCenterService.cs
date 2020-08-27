using System;
using Sers.Core.Module.Rpc;
using System.Collections.Generic;
using Vit.Core.Util.Pool;
using System.Threading;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;
using Sers.ServiceCenter.Entity;
using Newtonsoft.Json;

namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public abstract class ApiCenterService
    { 
 
        public void CallApiAsync(IOrganizeConnection  conn, Object sender, ArraySegment<byte> apiRequest, Action<object, List<ArraySegment<byte>>> callback)
        {
            CommunicationManageServer.CurConn = conn;

            var requestMessage = new ApiMessage(apiRequest);

            var rpcData = RpcFactory.CreateRpcContextData().UnpackOriData(requestMessage.rpcContextData_OriData);

            CallApiAsync(rpcData, requestMessage, sender, callback);
        }


        #region CallApi
        ObjectPoolGenerator<AutoResetEvent> pool_AutoResetEvent = new ObjectPoolGenerator<AutoResetEvent>(() => new AutoResetEvent(false));
        public bool CallApi(IOrganizeConnection  conn, ArraySegment<byte> apiRequest, out List<ArraySegment<byte>> replyData, int requestTimeoutMs)
        {
            List<ArraySegment<byte>> _replyData = null;

            AutoResetEvent mEvent = pool_AutoResetEvent.Pop();
            mEvent.Reset();

            CallApiAsync(conn, null, apiRequest, (sender, replyData_) =>
            {
                _replyData = replyData_;
                mEvent?.Set();
            });

            try
            {
                if (mEvent.WaitOne(requestTimeoutMs))
                {
                    replyData = _replyData;
                    return true;
                }
                else
                {
                    replyData = null;
                    return false;
                }
            }
            finally
            {
                var eToPush = mEvent;
                mEvent = null;
                pool_AutoResetEvent.Push(eToPush);
            }

        }
        #endregion


        #region interface

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        [JsonIgnore]
        public Action<IRpcContextData, ApiMessage> BeforeCallApi;

        public abstract void CallApiAsync(IRpcContextData rpcData, ApiMessage requestMessage, Object sender, Action<object, List<ArraySegment<byte>>> callback);



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
