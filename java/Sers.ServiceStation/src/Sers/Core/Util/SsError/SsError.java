package Sers.Core.Util.SsError;

import Sers.Core.Module.Api.Data.*;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDefaultValue;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDescription;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsExample;


public class SsError {

	/**
	 * 
	 */
	@SsDescription("")
	@SsExample("1000")
	@SsDefaultValue("")
    public int  errorCode;
	
	/**
	 * 
	 */
	@SsDescription("errorMessage")
	@SsExample("操作出现异常")
	@SsDefaultValue("")
    public String errorMessage;

    /**
     * 自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如："150721_lith_1"
     */
	@SsDescription("自定义ErrorTag格式。每处ErrorTag建议唯一。建议格式为 日期_作者缩写_自定义序号，例如：\"150721_lith_1\"")
	@SsExample("150721_lith_1")
	@SsDefaultValue("")
    public String errorTag;

    
    /**
     * 错误详情（json类型）
     */
	@SsDescription("错误详情（json类型）")
	@SsExample("{}")
	@SsDefaultValue("")
    public Object errorDetail;


    
    public SsError(){}
    
    public SsError(String errorMessage,int  errorCode){
    	this.errorMessage=errorMessage;
    	this.errorCode=errorCode;    	
    }
    
    public SsError(Exception ex){
    	this.errorMessage=ex.getCause().getMessage();
    	this.errorCode=100;    	
    }
    
    
    public ApiReturnBase toApiReturn() {
    	return new ApiReturnBase(this);
    }
    
    
    
    
    
    //region const
    


    /**
     * 100 系统出错
     */
    public static final SsError Err_SysErr = new SsError("系统出错",100 );    
    

    /**
     * 101 请求的api不存在
     */
    public static final SsError Err_ApiNotExists = new SsError("请求的api不存在",101 );    
    

    /**
     * 102 请求超时，无回应数据
     */
    public static final SsError Err_Timeout = new SsError("请求超时，无回应数据",102 );    
    

    /**
     * 110 服务限流限制
     */
    public static final SsError Err_RateLimit_Refuse = new SsError("服务限流限制",110 );    
    

    /**
     * 120 请求参数不合法
     */
    public static final SsError Err_InvalidParam = new SsError("请求参数不合法",120 );    
    

    /**
     * 404 404 Not Found：请求资源不存在
     */
    public static final SsError Err_404 = new SsError("404 Not Found：请求资源不存在",404 );  
    

    /**
     * 405 权限限制(没有权限)
     */
    public static final SsError Err_NotAllowed = new SsError("权限限制(没有相应权限)",405 );       
    
    
    //endregion
 
}
