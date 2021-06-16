package Sers.Core.Module.Api;

import java.lang.reflect.Type;
import java.util.List;

import Sers.Core.Module.Log.Logger;
import Sers.Core.Module.Message.ApiMessage;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Data.ArraySegment;
import Sers.Core.Util.SsError.SsError;

public class ApiClient {

	
	public interface IOnSendRequest{
		ArraySegment SendRequest(List<ArraySegment> requestData);
	}
	
	

	
    //region ApiClient

	public IOnSendRequest OnSendRequest;



    //region CallApi 原始

 
    public ArraySegment  CallApi(List<ArraySegment> reqOri)
    {
        return OnSendRequest.SendRequest(reqOri);
    }

 
    public ApiMessage CallApi(ApiMessage request)
    {
        try
        {
        	ArraySegment reply = CallApi(request.Package());
            if (null == reply || reply.count == 0)
            {
                //Logger.Error(new Exception().SsError_Set(SsError.Err_Timeout));
                //返回请求超时，无回应数据
                return new ApiMessage().InitByApiReturn(SsError.Err_Timeout.toApiReturn());
            }
            return new ApiMessage(reply);
        }
        catch (Exception ex)
        {         
            Logger.Error(ex); 
            return new ApiMessage().InitByApiReturn(new SsError(ex).toApiReturn());
        }
    }

    //endregion


    //region CallApi 扩展
 
   
	
	
	public ArraySegment CallApiToBytes(String route, Object arg) {
		ApiMessage apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);

		ApiMessage apiReplyMessage = CallApi(apiRequestMessage);

		ArraySegment replyData = apiReplyMessage.value_OriData_Get();

		return replyData;
	}
  
	public String CallApi(String route, Object arg) { 

		ArraySegment replyData = CallApiToBytes(route,arg);

		return Serialization.Instance.bytesToString(replyData);
	}
    
	
	 public <ReturnType>ReturnType CallApi(String route, Object arg,Class<ReturnType> clazz)
	    {
	    	ArraySegment replyData = CallApiToBytes(route,arg);
	    	
	        return Serialization.Instance.deserializeFromBytes(replyData,clazz);
	    }
	 
	 
	 
    public <ReturnType> ReturnType CallApi(String route, Object arg,Type clazz)
    {
    	String reply = CallApi(route,arg);

        return Serialization.Instance.deserializeFromString(reply,clazz);
    }
    
    
    //endregion



    //endregion
    
    
    
    public static final ApiClient Instance = new ApiClient();
    
    
}
