package Sers.Core.Module.Message;

import java.util.ArrayList;
import java.util.List;

import Sers.Core.Module.Api.Data.ApiReturn;
import Sers.Core.Module.Api.Data.ApiReturnBase;
import Sers.Core.Module.Rpc.RpcContextData;
import Sers.Core.Module.Rpc.RpcFactory;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Data.ArraySegment;

public class ApiMessage extends SersFile {

	public ApiMessage() {
	}

	public ApiMessage(ArraySegment oriData) {
		super(oriData);
	}

	@Override
	public List<ArraySegment> Files_Get() {

		if (Files == null) {
			Files = new ArrayList<ArraySegment>();
			Files.add(null);
			Files.add(null);
		}
		return Files;
	}

	
	public ArraySegment rpcContextData_OriData_Get() {
		return Files_Get().get(0);
	}
	
	public void rpcContextData_OriData_Set(ArraySegment value) {
		Files_Get().set(0,value);
	}
	
	
	
	public ArraySegment value_OriData_Get() {
		return Files_Get().get(1);
	}
	
	public void value_OriData_Set(ArraySegment value) {
		Files_Get().set(1,value);
	}
	
	
	
	//region 扩展 InitByApiReturn InitAsApiRequestMessage
	  public ApiMessage InitByApiReturn(ApiReturnBase ret)
      {          
		  byte[] bytes=Serialization.Instance.serializeToBytes(ret);
          value_OriData_Set(new ArraySegment(bytes));
          return this;
      }
	
	  

      public ApiMessage InitAsApiRequestMessage(String route, Object arg)
      {
    	  byte[] argBytes=Serialization.Instance.serializeToBytes(arg);
    	  
    	  value_OriData_Set(new ArraySegment(argBytes)); 

          RpcContextData rpcData = RpcFactory.Instance.CreateRpcContextData.create().InitFromRpcContext();

       
          rpcData.route_Set(route);
          rpcData.http_url_Set(route);
          
          rpcContextData_OriData_Set(rpcData.PackageOriData());

          return this;
      }
	//endregion
	 
	
}
