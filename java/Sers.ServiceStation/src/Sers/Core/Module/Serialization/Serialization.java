package Sers.Core.Module.Serialization;

import Sers.Core.Util.Data.ArraySegment;
import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;

import java.nio.charset.Charset;

public class Serialization {

    public  final static Serialization Instance=new Serialization();


    public Serialization(){
        charset= Charset.forName("UTF-8");
    }

    Charset charset;



    public String bytesToString(ArraySegment bytes){
        return new String(bytes.Array,bytes.Offset,bytes.Count,charset);
    }

    public byte[] stringToBytes(String value){
        return value.getBytes(charset);
    }


    Gson gson = new Gson();
    public String  SerializeToString(Object model){
        return   gson.toJson(model); // {"name":"张三kidou","age":24}
    }

    public byte[]    Serialize(Object model) throws JsonSyntaxException {
        return  stringToBytes(gson.toJson(model));
    }


    public <T> T   Deserialize(String json, Class<T> classOfT) throws JsonSyntaxException {
        return   gson.fromJson(json,classOfT);
    }

    public <T> T   Deserialize(ArraySegment bytes, Class<T> classOfT) throws JsonSyntaxException {
        return   gson.fromJson(bytesToString(bytes),classOfT);
    }


}
