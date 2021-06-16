package Sers.Core.Module.Api.LocalApi;

import java.io.IOException;
import java.util.Collection;
import java.util.HashMap;
import java.util.Map;

import Sers.Core.Module.Api.Data.ApiReturnBase;
import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Message.ApiMessage;
import Sers.Core.Module.Rpc.RpcContextData;
import Sers.Core.Module.Rpc.RpcContext;
import Sers.Core.Module.Rpc.RpcFactory;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Module.SersDiscovery.DiscoveryConfig;
import Sers.Core.Module.SsApiDiscovery.LocalApiNode;
import Sers.Core.Module.SsApiDiscovery.SsApiDiscovery;
import Sers.Core.Util.ConfigurationManager.ConfigurationManager;
import Sers.Core.Util.Data.ArraySegment;
import Sers.Core.Util.SsError.SsError;

public class LocalApiMng {
	public Map<String, LocalApiNode> apiMap = new HashMap<String, LocalApiNode>();

	SsApiDiscovery apiMng = new SsApiDiscovery(apiMap);
	
	public LocalApiNode[] getApiNodes() {

		return apiMap.values().stream().toArray(LocalApiNode[]::new);
	}

	/**
	 * 发现服务,可多次调用
	 * @param config
	 * @throws Exception
	 */
	public void discovery(DiscoveryConfig config) throws Exception {
		apiMng.discovery(config);
	}

	/**
	 * 发现服务,可多次调用
	 * @param packageName
	 * @throws Exception
	 */
	public void discovery(String packageName)  {
		DiscoveryConfig config = new DiscoveryConfig();
		config.packageName = packageName;
	}
	
	/**
	 * 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
	 * @throws Exception 
	 */
	public void discovery() throws Exception  {
		DiscoveryConfig[] configs=ConfigurationManager.Instance.GetByPath("Sers.ApiStation.DiscoveryConfig",DiscoveryConfig[].class);
		if(configs==null || configs.length==0)
			return;
		for(DiscoveryConfig config : configs) {
			discovery(config);
		}		
	}
	
	
	
	
	// region CallLocalApi

	/**
	 * 构建RpcContext并调用
	 * 
	 * @param apiRequest
	 * @return
	 * @throws IOException
	 */
	public ApiMessage CallLocalApi(ApiMessage apiRequest) {
		try (RpcContext rpcContext = RpcFactory.Instance.CreateRpcContext.create()) {
			try {
				rpcContext.apiRequestMessage = apiRequest;
				rpcContext.apiReplyMessage = new ApiMessage();

				RpcContextData rpcData = RpcFactory.Instance.CreateRpcContextData.create();
				rpcData.UnpackOriData(apiRequest.rpcContextData_OriData_Get());
				rpcContext.rpcData = rpcData;

				String route = rpcData.route_Get();
				LocalApiNode apiNode = apiMap.get(route);

				if (null == apiNode) {
					ApiReturnBase apiRet = new ApiReturnBase(new SsError("api not found! route:" + route, 100));
					rpcContext.apiReplyMessage
							.value_OriData_Set(new ArraySegment(Serialization.Instance.serializeToBytes(apiRet)));
				} else {
					byte[] replyBytes = apiNode.invoke(apiRequest.value_OriData_Get());
					rpcContext.apiReplyMessage.value_OriData_Set(new ArraySegment(replyBytes));
				}

			} catch (Exception ex) {

				Logger.Error(ex);

				ApiReturnBase apiRet = new ApiReturnBase(new SsError(ex));
				rpcContext.apiReplyMessage
						.value_OriData_Set(new ArraySegment(Serialization.Instance.serializeToBytes(apiRet)));
			}
			return rpcContext.apiReplyMessage;
		}
	}
	// endregion

}
