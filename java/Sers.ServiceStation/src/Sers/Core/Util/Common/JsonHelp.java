package Sers.Core.Util.Common;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonPrimitive;
import com.google.gson.JsonSyntaxException;

public class JsonHelp {

	
	//region get
	/**
	 * return null if empty
	 * @param json
	 * @param path
	 * @return
	 */
	public static  JsonElement ElementGetByPath(JsonObject json,String...  path)
    {    	 
    	JsonElement cur=json;
    	for(String name :  path) {
    		if(cur==null || !cur.isJsonObject()) return null;
    		cur=cur.getAsJsonObject().get(name);
    	}
    	return cur;
    }
	
	
	 
	 
    public static String StringGetByPath(JsonObject json,String ... path)
    {    	 
    	JsonElement cur= ElementGetByPath(json, path);   
    	if(cur==null|| cur.isJsonNull()) 
    		return null;
    	
    	if(cur.isJsonPrimitive()) 
    		return cur.getAsString();
    	
    	return cur.toString();
    }

    
    
    public static <T> T   ValueGetByPath(JsonObject json, Class<T> classOfT,String ... path) throws JsonSyntaxException {
    	JsonElement cur = ElementGetByPath(json, path);   
    	if(cur==null) return null;
    	return new Gson().fromJson(cur, classOfT);
    }
    
    //endregion
    
    
    
    
    
    //region set
    
    public static void ValueSetByPath(JsonObject json, String value,String ... path) {
    	JsonPrimitive elem=new JsonPrimitive(value);
    	ValueSetByPath(json,elem,path);
	}
    
    
    public static void ValueSetByPath(JsonObject json, JsonElement value,String ... path) {
		 
		JsonObject cur=json;
		
		int t=0;
		String name;
    	for(;t<path.length-1; t++) {
    		name=path[t];	    		
    		
    		JsonElement child=cur.get(name);
    		if(child!=null && child.isJsonObject()) {
    			cur=child.getAsJsonObject();	    			
    		} else {
    			JsonObject childNew=new JsonObject();
    			cur.add(name, childNew);
    			cur=childNew;
    		}
    	}		
    	
    	name=path[t];	
    	cur.remove(name);
    	
    	cur.add(name, value);
	}
    
    //endregion
    
    
    
    
    
    
    
    
    
    
    
    
    
}
