using Sers.Core.Module.Api.Message;
using System;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System.Collections.Generic;
using Sers.Core.Util.Pool;
using System.Threading;

namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public class ApiCenterService
    {
 

        IApiCenterManage apiCenterManage;

        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>        
        public Action<IRpcContextData, ApiMessage> BeforeCallApi { get => apiCenterManage.BeforeCallApi; set => apiCenterManage.BeforeCallApi = value; }


        public void SetApiCenterManage(IApiCenterManage apiCenterManage)
        {
            this.apiCenterManage = apiCenterManage;
        }


        public void ServiceStation_Regist(ServiceStation serviceStation)
        {
            apiCenterManage.ServiceStation_Regist(serviceStation);
        }

        public void ServiceStation_Remove(IMqConn mqConn)
        {
            apiCenterManage.ServiceStation_Remove(mqConn);
        }
 
 
        public void CallApiAsync(IMqConn mqConn, Object sender, ArraySegment<byte> apiRequest, Action<object, List<ArraySegment<byte>>> callback)
        {
            ServerMqManager.CurMqConn = mqConn;

            var requestMessage = new ApiMessage(apiRequest);

            var rpcData = RpcFactory.Instance.CreateRpcContextData().UnpackOriData(requestMessage.rpcContextData_OriData);

            apiCenterManage.CallApiAsync(rpcData, requestMessage, sender, callback);

        }


        #region CallApi
        ObjectPoolGenerator<AutoResetEvent> pool_AutoResetEvent = new ObjectPoolGenerator<AutoResetEvent>(() => new AutoResetEvent(false));
        public bool CallApi(IMqConn mqConn, ArraySegment<byte> apiRequest, out List<ArraySegment<byte>> replyData, int requestTimeoutMs)
        {
            List<ArraySegment<byte>> _replyData = null;

            AutoResetEvent mEvent = pool_AutoResetEvent.Pop();
            mEvent.Reset();

            CallApiAsync(mqConn, null, apiRequest, (sender, replyData_) =>
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





    }
}
