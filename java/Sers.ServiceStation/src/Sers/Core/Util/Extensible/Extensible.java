package Sers.Core.Util.Extensible;

import java.util.HashMap;
import java.util.Map;

public class Extensible {
	
	private Map<String, Object> extensionData = new HashMap<>();
	
	
	  public void setData(String key,Object value)
      {
		  if(extensionData.containsKey(key)) {			  
			  extensionData.replace(key, value);
		  }else {
			  extensionData.put(key, value);
		  }	 
      }
	  
	  public <T> T getData(String key)
      {
		  return (T)extensionData.getOrDefault(key, null);
      }	

}
