using System;
using Sers.Core.Module.Rpc;
using System.Threading;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;
using Sers.ServiceCenter.Entity;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Vit.Core.Util.Threading;

namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public abstract class ApiCenterService
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(IOrganizeConnection  conn, Object sender, ArraySegment<byte> apiRequest, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        {
            CommunicationManageServer.CurConn = conn;

            var requestMessage = new ApiMessage(apiRequest);

            CallApiAsync(requestMessage, sender, callback);
        }


        #region CallApi


        #region static curAutoResetEvent      
        public static AutoResetEvent curAutoResetEvent =>
            _curAutoResetEvent.Value ?? (_curAutoResetEvent.Value = new AutoResetEvent(false));

        static System.Threading.ThreadLocal<AutoResetEvent> _curAutoResetEvent = new System.Threading.ThreadLocal<AutoResetEvent>();
        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CallApi(IOrganizeConnection  conn, ArraySegment<byte> apiRequest, out Vit.Core.Util.Pipelines.ByteData replyData, int requestTimeoutMs)
        {
            Vit.Core.Util.Pipelines.ByteData _replyData = null;

            AutoResetEvent mEvent = curAutoResetEvent;
            mEvent.Reset();

            CallApiAsync(conn, null, apiRequest, (sender, replyData_) =>
            {
                _replyData = replyData_;
                mEvent?.Set();
            });

            bool success;
            try
            {
                success = mEvent.WaitOne(requestTimeoutMs);
            }
            finally
            {
                mEvent = null;
            }

            if (success)
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
        #endregion


        #region interface

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        [JsonIgnore]
        public Action<RpcContextData, ApiMessage> BeforeCallApi;

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
