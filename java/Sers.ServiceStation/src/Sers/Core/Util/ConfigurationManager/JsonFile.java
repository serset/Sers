package Sers.Core.Util.ConfigurationManager;


import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.google.gson.JsonSyntaxException;

import Sers.Core.Util.Common.CommonHelp;

public class JsonFile {

	protected JsonObject root;
    protected String configPath;
    public JsonFile(String configPath)
    {
        this.configPath = configPath;
        RefreshConfiguration();
    }

    /// <summary>
    /// 相对路径
    /// </summary>
    /// <param name="configPath"></param>
    public JsonFile(String ... configPath)
    {
    	this(CommonHelp.GetAbsPathByRealativePath(configPath));
    
    }

    
    
    /// <summary>
    /// 手动刷新配置，修改配置后，请手动调用此方法，以便更新配置参数
    /// </summary>
    public  void RefreshConfiguration()
    {
    	root = null;

        //region (x.1)解析Json文件,失败返回null
               
        try
        {
        	String fileContent = CommonHelp.File_ReadAllText(configPath);    
        	if(!CommonHelp.StringIsNullOrEmpty(fileContent)) {
        		root=new JsonParser().parse(fileContent).getAsJsonObject();
        	}        	 
        }
        catch(Exception ex) { }
        //endregion

        //(x.2)
        if (root == null)
            root = new JsonObject();        
   
    }
    
 
    private JsonElement GetByPath(String  path)
    {    	 
    	JsonElement cur=root;
    	for(String name :  path.split("\\.")) {
    		if(cur==null || !cur.isJsonObject()) return null;
    		cur=cur.getAsJsonObject().get(name);
    	}
    	return cur;
    }
    
    public String GetStringByPath(String path)
    {    	 
    	JsonElement cur=GetByPath(path); 
    	if(cur==null|| cur.isJsonNull()) 
    		return null;
    	
    	if(cur.isJsonPrimitive()) 
    		return cur.getAsString();
    	
    	return cur.toString();
    }

    public <T> T   GetByPath(String path, Class<T> classOfT) throws JsonSyntaxException {
    	JsonElement cur=GetByPath(path); 
    	if(cur==null) return null;
    	return new Gson().fromJson(cur, classOfT);
    }
     


}
