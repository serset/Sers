package Sers.ServiceStation;

import java.io.Closeable;
import java.io.IOException;
import java.lang.reflect.Modifier;
import java.util.Collection;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

import Sers.Core.Module.Api.ApiClient;
import Sers.Core.Module.Api.Data.ApiReturnBase;
import Sers.Core.Module.Api.LocalApi.LocalApiMng;
import Sers.Core.Module.App.SersApplication;
import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Message.ApiMessage;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Module.SersDiscovery.DiscoveryConfig;
import Sers.Core.Module.SsApiDiscovery.LocalApiNode;
import Sers.Core.Module.SsApiDiscovery.SsApiDiscovery;
import Sers.Core.Util.ConfigurationManager.ConfigurationManager;
import Sers.Mq.SocketMq.ClientMq;
import Sers.Mq.SocketMq.ClientMqConfig;

public class ServiceStation implements Closeable {

	public static final ServiceStation Instance=new ServiceStation();
	
	ClientMq mq=new ClientMq();

    LocalApiMng localApiMng = new LocalApiMng();
     
	//region (x.1) InitStation
    public void InitStation()
    {
    	 Logger.Info("初始化ServiceStation...");
    	 
    	 if ("true" == ConfigurationManager.Instance.GetStringByPath("Sers.ServiceStation.Console_PrintLog"))
         {
             Logger.onLog = (String level,String msg)->{
                 System.out.println("["+level+"]"+msg);
             };
         } 
    	 
    	 mq.config=ConfigurationManager.Instance.GetByPath("Sers.Mq.Socket",ClientMqConfig.class);
    	 
//
//
//
//        mqMng.UseSocket();
//
//        localApiMng.UseSsApiDiscovery();
//
//        localApiMng.UseSubscriberDiscovery();
//
//        localApiMng.UseApiTrace();
//
//        UseUsageReporter();

    }
    //endregion
    
    
    //region (x.2) StartStation
    public boolean StartStation() {
    	
    
    	 
    	 
        SersApplication.IsRunning = false;
        
        //region (x.1) 注册 Mq 回调

        mq.OnReceiveRequest_Set( (oriData) ->
        {
        	ApiMessage apiRequestMessage=new ApiMessage(oriData);
        	ApiMessage apiReplyMessage = localApiMng.CallLocalApi(apiRequestMessage);
            return apiReplyMessage.Package();
        });

        mq.OnDisconnected_Set( (obj) ->
        {
        	close();
        });
        //endregion
        
        
        
        //region (x.2) ClientMq 连接服务器
        Logger.Info("[ClientMq] 准备连接服务器");        
        
        mq.config = ConfigurationManager.Instance.GetByPath("Sers.Mq.Socket",ClientMqConfig.class);
        
        if (!mq.Connect())
        {
            Logger.Info("[ClientMq] 连接服务器 失败");
            return false;
        }

        Logger.Info("[ClientMq] 连接服务器 成功");
        //endregion
        
        

        //(x.3) 初始化ApiClient
        ApiClient.Instance.OnSendRequest =  (requestData)->{  			 
			return mq.SendRequest(requestData);			 	
		};
		
		
        //region (x.4)向服务中心注册ServiceStation
//        if (0 < localApiMng.map.size())
        {
            Logger.Info("向服务中心注册ServiceStation...");
            
       	 	String serviceStationData = BuildServiceStationRegistArg();
      

            if ("true".equals(ConfigurationManager.Instance.GetStringByPath("Sers.ServiceStation.StationRegist_PrintRegistArg")))
            {
                Logger.Info("[StationRegist] arg:" + serviceStationData);
            }


            ApiReturnBase ret;
            try
            {
                ret = ApiClient.Instance.CallApi("/_sys_/serviceStation/regist", serviceStationData,ApiReturnBase.class);
            }
            catch (Exception ex)
            {
                Logger.Error("向服务中心注册本地Api 失败", ex);
                return false;
            }

            if (!ret.success)                
            {
                Logger.Info("向服务中心注册本地Api 失败。返回结果：" + Serialization.Instance.serializeToString(ret));
                return false;
            }

            Logger.Info("向服务中心注册本地Api 成功");

        }
        //endregion
      
        
        Logger.Info("ServiceStation启动成功。StationName:"+ ConfigurationManager.Instance.GetStringByPath("Sers.ServiceStation.serviceStationInfo.serviceStationName"));
        SersApplication.IsRunning = true;
     
        return true;
    }
   
     
    
    private String BuildServiceStationRegistArg() {
    	
    	
    	JsonObject arg=new JsonObject();
    	
    	arg.add("serviceStationInfo", ConfigurationManager.Instance.GetByPath("Sers.ServiceStation.serviceStationInfo",JsonObject.class));
    	
    	arg.add("deviceInfo", ConfigurationManager.Instance.GetByPath("Sers.ServiceStation.deviceInfo",JsonObject.class));
    	
    	arg.add("apiNodes", Serialization.Instance.convertBySerialize( localApiMng.getApiNodes() , JsonArray.class));
 
    	return arg.toString();
    }
    
    //endregion
    
    
    
 
    
    /**
	 * 发现服务,可多次调用
	 * @param config
	 * @throws Exception
	 */
	public void Discovery(DiscoveryConfig config) throws Exception {
		localApiMng.discovery(config);	
	}

	/**
	 * 发现服务,可多次调用
	 * @param packageName
	 * @throws Exception
	 */
	public void Discovery(String packageName)  {
		localApiMng.discovery(packageName);	
	}
	
	/**
	 * 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
	 * @throws Exception 
	 */
	public void Discovery() throws Exception  {
		localApiMng.discovery();			
	}
    



	@Override
	public void close()  {
		 
		 if (!SersApplication.IsRunning) return;
         Logger.Info("站点关闭...");

         //Mq Close
         try
         {
             mq.close();
         }
         catch (Exception ex)
         {
             Logger.Error(ex);
         }
         

         Logger.Info("站点已关闭");

         SersApplication.Stop();
		
	}
	
	
	public void RunAwait() {
		
		try {
			SersApplication.RunAwait();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			Logger.Error(e);
		}
		
	}
	
    
    
}
