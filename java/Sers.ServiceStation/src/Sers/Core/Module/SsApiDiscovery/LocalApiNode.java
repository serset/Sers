package Sers.Core.Module.SsApiDiscovery;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import Sers.Core.Module.Api.ApiDesc.SsApiDesc;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Data.ArraySegment;

public class LocalApiNode {	

	private Method method;
	private IApiController controller;

	public SsApiDesc apiDesc;
	// public String route;
	
	public void  init(Method method,IApiController controller) {
		this.method=method;
		this.controller=controller;		
	}
	

	
	public byte[] invoke(ArraySegment arg_OriData) throws  Exception {
		
		 //(x.1)反序列化 请求参数
        Object[] args = apiDesc.argType.deserialize(arg_OriData);

        //(x.2) Invoke
        Object returnValue=method.invoke(controller,args);

        //(x.3) 序列化 返回数据
        return Serialization.Instance.serializeToBytes(returnValue);   
    
	}

}
