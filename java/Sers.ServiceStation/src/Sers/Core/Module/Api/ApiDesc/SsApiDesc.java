package Sers.Core.Module.Api.ApiDesc;

import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsExample;
import Sers.Core.Module.SsApiDiscovery.SersValid.SsValidation;

public class SsApiDesc {

    /**
     * api名称(不为route)
     */ 
    public String name;


    /**
     * 文字描述
     */    
    public String description;

   
    /**
     * 路由 例如 "/ApiStation1/path1/path2/api1"
     */  
    @SsExample("/ApiStation1/path1/path2/api1")
    public String route;

   
    /**
     * 请求参数类型   SsModel类型
     */
    public SsModel argType;
 
    /**
     * 返回数据类型   SsModel类型
     */    
    public SsModel returnType;



 
	/**
     * api调用限制
     */  
    public SsValidation[] rpcValidations;
 


    /**
     * 额外数据
     */  
    public Object ext;


    public String getApiStationName()
    {
        try
        {
            return route.split("\\/")[1];
        }
        catch (Exception e)
        {                
        }
        return null;
    }

}
