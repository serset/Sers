package Sers.Core.Module.Rpc;

import java.io.Closeable;
import java.io.IOException;
import java.util.List;

import Sers.Core.Module.Message.ApiMessage;
import Sers.Core.Util.Threading.AsyncCache;

public class RpcContext implements Closeable {

	 //region static

     static AsyncCache<RpcContext> CurrentRpcContext_AsyncCache = new AsyncCache<RpcContext>();

     public static RpcContext Current_Get() {
    	 return CurrentRpcContext_AsyncCache.get(); 
     }

     public static RpcContextData RpcData_Get() { 
    	 RpcContext current=Current_Get();
    	 return current==null?null: current.rpcData;    	 
     }

     //endregion


     
     
     public ApiMessage apiRequestMessage;
     public ApiMessage apiReplyMessage;

     public RpcContextData rpcData ;
 
     
     //region 构造函数 和 close
     
     private List<Closeable> onEnds;
     
     public RpcContext() {
    	 CurrentRpcContext_AsyncCache.set(this);
     }
    
     
	@Override
	public void close() {
		// TODO Auto-generated method stub
		
		 if (CurrentRpcContext_AsyncCache.get() == this)
         {
             CurrentRpcContext_AsyncCache.set(null);
         }
		
	}
	
	//endregion

}
