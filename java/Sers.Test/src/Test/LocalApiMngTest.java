package Test;

import Sers.Core.Module.Api.LocalApi.LocalApiMng;
import Sers.Core.Module.SersDiscovery.DiscoveryConfig;
import Sers.Core.Module.Reflection.ReflectionHelp;
import Sers.Core.Module.SersDiscovery.DiscoveryConfig;
import Sers.Core.Module.SsApiDiscovery.LocalApiNode;
import Sers.Core.Module.SsApiDiscovery.SsApiDiscovery;
import Sers.Core.Util.Data.ArraySegment;
import Test.JsonTest;
import Test.LoggerTest;
import Test.ReflectionTest;
import Test.SocketMqTest;

import java.util.HashMap;
import java.util.Map;

import com.google.gson.Gson;


public class LocalApiMngTest {

	

    public static void Test() {

    	int size;
    	try {
//			ReflectionTest.Test();
    		LocalApiMng localApiMng= new LocalApiMng();
    		
    		DiscoveryConfig config=new DiscoveryConfig();    		
    		config.packageName=("StationDemo.Controllers");
    		
    		
    		localApiMng.discovery(config);
    		
    		size=localApiMng.apiMap.size();
    		
    		String arg="{\"name1\":4,\"name3\":5}";
    		String ret;
    		
    		byte[]bytesArg=new byte[] {4,5,6};
//    		byte[]bytesRet=localApiMng.CallLocalApi("/DemoStation/v20/demo/3", bytesArg, byte[].class);
//    		
//    		bytesRet=localApiMng.CallLocalApi("/DemoStation/v20/demo/4", bytesArg, byte[].class);
//    		
//    		ret=localApiMng.CallLocalApi("/DemoStation/v20/demo/2", arg, String.class);
   		
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	 

    }
    
    
	// region CallLocalApi

//	public static ArraySegment CallLocalApi(LocalApiMng localApiMng,String route, ArraySegment arg) throws Exception {
//
//		LocalApiNode apiNode = map.get(route);
//		return new ArraySegment(apiNode.invoke(arg));
//	}
//
//	public <ReturnType> ReturnType CallLocalApi(String route, Object arg, Class<ReturnType> classOfT) throws Exception {
//		byte[] argBytes = Serialization.Instance.serializeToBytes(arg);
//		ArraySegment argSeg = new ArraySegment(argBytes);
//		ArraySegment returnValue = CallLocalApi(route, argSeg);
//		return Serialization.Instance.deserializeFromBytes(returnValue, classOfT);
//	}

	// endregion
    
}
