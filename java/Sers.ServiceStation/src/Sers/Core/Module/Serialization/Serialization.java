package Sers.Core.Module.Serialization;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Util.Data.ArraySegment;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonElement;
import com.google.gson.JsonSyntaxException;

import java.lang.reflect.Modifier;
import java.lang.reflect.Type;
import java.nio.charset.Charset;

public class Serialization {

    public  final static Serialization Instance=new Serialization();


    
    Charset charset;
    Gson gson = GetDefaultGson();
    
    private static Gson GetDefaultGson() {
    	
    	return new GsonBuilder().excludeFieldsWithModifiers(Modifier.PRIVATE,Modifier.PROTECTED,Modifier.STATIC)    			 
//				.excludeFieldsWithoutExposeAnnotation()
//				.setPrettyPrinting()
				.create();
    }
    
    public Serialization(){
        charset= Charset.forName("UTF-8");
    }
    
    
    //region bytes <--> String

    public String bytesToString(ArraySegment bytes){
        return new String(bytes.array,bytes.offset,bytes.count,charset);
    }

    public byte[] stringToBytes(String value){
        return value.getBytes(charset);
    }

    //endregion
 
    
    
    
    
    //region SerializeToString
    
    public String  serializeToString(Object model){
//    	try {
    		 return   gson.toJson(model); // {"name":"张三kidou","age":24}
    		
//    	}catch(Exception ex) {
//    		Logger.Error(ex);
//    	}
//    	
//       return  null;
    }
    
    //endregion
    
    
    //region DeserializeFromString  
    
    public <T> T   deserializeFromString(String json, Class<T> classOfT) throws JsonSyntaxException {     
        return   gson.fromJson(json,classOfT);
    }
    
    public <ReturnType> ReturnType   deserializeFromString(String data, Type classOfT) throws JsonSyntaxException {    	
        return   gson.fromJson(data,classOfT);
    }
 
 
    public <T> T   deserialize(JsonElement json, Class<T> classOfT) throws JsonSyntaxException {     
        return   gson.fromJson(json,classOfT);
    }
    //endregion
    
    
    
    //region SerializeToBytes

    public byte[]    serializeToBytes(Object model) throws JsonSyntaxException {
    	if(null==model) return null;
    	
    	Class clazz=model.getClass();
    	if(clazz.isAssignableFrom(byte[].class)) {
    		return ( byte[])model;
    	}
    	if(clazz.isAssignableFrom(ArraySegment.class)) {
    		return ((ArraySegment)model).ToBytes();
    	}
    	
    	String str;
    	
    	if(clazz.isAssignableFrom(String.class)) {
    		str= (String)model;
    	}else {
    		str=gson.toJson(model);
    	}
    	
        return  stringToBytes(str);
    }
   
    //endregion
    
    
    //region DeserializeFromBytes

    public <T> T   deserializeFromBytes(ArraySegment bytes, Class<T> classOfT) throws JsonSyntaxException {
    	
    	
    	if(byte[].class.isAssignableFrom(classOfT)) {
    		return (T)bytes.ToBytes();
    	}
    	
    	if(ArraySegment.class.isAssignableFrom(classOfT)) {
    		return (T)bytes;
    	}
    	
    	String str=bytesToString(bytes);
    	if(String.class.isAssignableFrom(classOfT)) {
    		return (T)str;
    	}
    	
        return   deserializeFromString(str,classOfT);
    }
    
 
    //endregion
    

   
    
 
    //endregion
    
    
    
    //region ConvertBySerialize
    
    public <T> T convertBySerialize(Object value, Class<T> classOfT)throws JsonSyntaxException {
    	 String str = serializeToString(value);
         return deserializeFromString(str,classOfT);
    }
    //endregion

}
