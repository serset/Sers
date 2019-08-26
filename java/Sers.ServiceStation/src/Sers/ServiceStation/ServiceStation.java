package Sers.ServiceStation;

import Sers.Core.Module.App.SersApplication;
import Sers.Core.Module.Log.Logger;
import Sers.Core.Util.ConfigurationManager.ConfigurationManager;
import Sers.Mq.SocketMq.ClientMq;
import Sers.Mq.SocketMq.ClientMqConfig;

public class ServiceStation {

	
	
	ClientMq mq=new ClientMq();


	//region
    public void InitStation()
    {
    	
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
    
    
    //region (x.2)
    public boolean StartStation() {
    	
        SersApplication.IsRunning = false;
    	
    	return false;
    }
    //endregion
    
    
}
