using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiMng
    {

        public LocalApiMng()
        { 
        }

        public void Discovery(Assembly assembly, DiscoveryConfig config = null)
        {
            discoveryMng.Discovery(assembly, config);
        }

        public void Discovery(DiscoveryConfig config)
        {
            discoveryMng.Discovery(config);
        }

        public readonly SersDiscoveryMng discoveryMng = new SersDiscoveryMng();


        /// <summary>
        /// 映射  route -> LocalApiNode
        /// </summary>
        public readonly SortedDictionary<string, IApiNode> apiMap = new SortedDictionary<string, IApiNode>();

        public IEnumerable<IApiNode> apiNodes => apiMap.Select((kv) => kv.Value);

        /// <summary>
        /// api的个数
        /// </summary>
        public int apiCount =>  apiMap.Count;





        #region CallLocalApi
        /// <summary>
        /// 构建RpcContext并调用
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public ApiMessage CallLocalApi(ApiMessage apiRequest)
        {
            using (var rpcContext = RpcFactory.Instance.CreateRpcContext())
            //using (var iocScope = IocHelp.CreateScope())
            {
                try
                {
                    rpcContext.apiRequestMessage = apiRequest;
                    rpcContext.apiReplyMessage = new ApiMessage();

                    var rpcData = RpcFactory.Instance.CreateRpcContextData();
                    rpcData.UnpackOriData(apiRequest.rpcContextData_OriData);
                    rpcContext.SetRpcContextData(rpcData);


                    apiMap.TryGetValue(rpcData.route, out var apiNode);
                    if (null == apiNode)
                    {
                        ApiReturn apiRet = new SsError { errorMessage = "api not found! route:" + rpcData.route, errorCode = 100 };
                        rpcContext.apiReplyMessage.value_OriData = Serialization.Serialization.Instance.Serialize(apiRet).BytesToArraySegmentByte();
                    }
                    else
                    {
                        rpcContext.apiReplyMessage.value_OriData = apiNode.Invoke(apiRequest.value_OriData).BytesToArraySegmentByte();
                    }

                }
                catch (Exception ex)
                {
                    ex = ex.GetBaseException();
                    Logger.Error(ex);
                    ApiReturn apiRet = ex;
                    rpcContext.apiReplyMessage.value_OriData = Serialization.Serialization.Instance.Serialize(apiRet).BytesToArraySegmentByte();
                }
                return rpcContext.apiReplyMessage;

            }
        }
        #endregion




        #region CallLocalApi

        public ArraySegment<byte> CallLocalApi(string route, Object arg)
        {
            var apiRequestMsg = new ApiMessage().InitAsApiRequestMessage(route, arg); 

            var apiReplyMessage = CallLocalApi(apiRequestMsg);

            return apiReplyMessage.value_OriData;
        }

        

        public ReturnType CallLocalApi<ReturnType>(string route, Object arg)
        {
            var returnValue = CallLocalApi(route, arg);
            return Serialization.Serialization.Instance.Deserialize<ReturnType>(returnValue);
        }
        
        

        #endregion

     

 


       
    }
}
