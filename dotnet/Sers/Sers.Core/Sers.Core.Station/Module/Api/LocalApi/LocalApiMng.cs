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
using Sers.Core.Util.Common;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.SsError;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiMng
    {

      

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


        public LocalApiMng()
        {
        }

        #region Discovery

        /// <summary>
        /// 发现服务,可多次调用
        /// </summary>
        /// <param name="config"></param>
        public void Discovery(DiscoveryConfig config)
        {

            //(x.1) load from dll file
            if (config.assembly == null && !String.IsNullOrEmpty(config.assemblyFile))
            {
                try
                {
                    config.assembly = Assembly.LoadFile(CommonHelp.GetAbsPathByRealativePath(config.assemblyFile));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            //(x.2) load by assemblyName
            if (config.assembly == null && !String.IsNullOrEmpty(config.assemblyName))
            {
                try
                {
                    config.assembly = Assembly.Load(config.assemblyName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }


            if (config.assembly == null) return;

            Logger.Info("Discovery,程序集:[" + config.assembly?.FullName + "]");
            discoveryMng.Discovery(config);
        }

        /// <summary>
        /// 发现服务,可多次调用
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void Discovery(Assembly assembly, DiscoveryConfig config = null)
        {
            if (null == config) config = new DiscoveryConfig();
            config.assembly = assembly;
            Discovery(config);
        }


        /// <summary>
        /// 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
        /// </summary>
        public void Discovery()
        {
            ConfigurationManager.Instance.GetByPath<List<DiscoveryConfig>>("Sers.ApiStation.DiscoveryConfig")?.ForEach(  config => Discovery(config)  );
        }




        #endregion

        #region CallLocalApi
        /// <summary>
        /// 构建RpcContext并调用
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public ApiMessage CallLocalApi(ApiMessage apiRequest)
        {
            using (var rpcContext = RpcFactory.Instance.CreateRpcContext())
            {
                try
                {
                    rpcContext.apiRequestMessage = apiRequest;
                    rpcContext.apiReplyMessage = new ApiMessage();

                    var rpcData = RpcFactory.Instance.CreateRpcContextData();
                    rpcData.UnpackOriData(apiRequest.rpcContextData_OriData);
                    rpcContext.rpcData=rpcData;


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

 


       
    }
}
