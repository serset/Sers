package Test;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

import Sers.Core.Util.ConfigurationManager.ConfigurationManager;
import Sers.Mq.SocketMq.ClientMqConfig;


  class User {
	
	 public String name="z";
	public int age=16;
}


public class JsonTest {
	public static void Test2() {
		
		User user=new User();
		 
		
		String string = new Gson().toJson(user);
	    JsonObject jsonObject = new Gson().toJsonTree(user).getAsJsonObject(); 
	    JsonObject jsonObject2 = new JsonParser().parse(string).getAsJsonObject();
	   
	    
	}
	
	//TestJsonFile
	public static void Test() {
		String Encoding2=ConfigurationManager.Instance.GetStringByPath("Sers.Serialization");
		String Encoding=ConfigurationManager.Instance.GetStringByPath("Sers.Serialization.Encoding");
		
		ClientMqConfig config=ConfigurationManager.Instance.GetByPath("Sers.Mq.Socket",ClientMqConfig.class);
	}

}
