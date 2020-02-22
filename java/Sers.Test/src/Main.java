import Sers.Core.Module.Api.LocalApi.LocalApiMng;
import Sers.Core.Module.Reflection.ReflectionHelp;
import Sers.Core.Module.SersDiscovery.DiscoveryConfig;
import Sers.Core.Module.SsApiDiscovery.LocalApiNode;
import Sers.Core.Module.SsApiDiscovery.SsApiDiscovery;
import Sers.Core.Util.ConfigurationManager.ConfigurationManager;
import Sers.Core.Util.Data.ArraySegment;
import Sers.ServiceStation.ServiceStation;
import Test.JsonTest;
import Test.LoggerTest;
import Test.ReflectionTest;
import Test.SocketMqTest;

import java.util.HashMap;
import java.util.Map;

import com.google.gson.Gson;




public class Main {





    public static void main(String[] args) {

    	
    	SocketMqTest.Test();    	
    	
    	  
//        byte[] bs=new byte[4];
//        ArraySegment ar=new  ArraySegment(bs);
//
//        int v=-600000;
//        ar.WriteInt32(v,0);
//
//        int v2=ar.ReadInt32(0);

//    	JsonTest.Test();
    	
//      LoggerTest.Test();
    	
//    	int size;
//    	try {
////			ReflectionTest.Test();
//    		 
//   		
//		} catch (Exception e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//    	 
    	
//    	//(x.1) init
//    	ServiceStation.Instance.InitStation();
//    	
//    	//(x.2) Discovery api
//    	try {
//			ServiceStation.Instance.Discovery();
//		} catch (Exception e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//			return;
//		}
//    	
//    	//(x.3) StartStation
//    	ServiceStation.Instance.StartStation();
//    	
//    	//(x.4) RunAwait
//    	ServiceStation.Instance.RunAwait();
    	
    	
    	
    	
//        System.out.println("Hello World!");
    }
}
