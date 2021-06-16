package Sers.Core.Module.SsApiDiscovery.SersValid;

import java.lang.reflect.Method;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import com.google.gson.JsonObject;

import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.SsError.SsError;

public class SersValidMng {
	 public static SsValidation[] getRpcValidationsFromMethod(Method  info)
     {     
		 List<SsValidation> validations;
	 
		 
		 //region (x.1) SsValidation
		 Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsValidation[] valids
		 = info.getAnnotationsByType(Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsValidation.class);
		 
		 validations= Arrays.stream(valids).map( anotation->{
			 SsValidation valid=new SsValidation();
			 
			 valid.path=anotation.path();
			 valid.ssError= new SsError(anotation.errorMessage(),anotation.errorCode());
			 valid.ssValid = Serialization.Instance.deserializeFromString(anotation.ssValid(), JsonObject.class);
			 return valid;			 
		 }).collect(Collectors.toList());
		 //endregion
		 
	 
		 //region (x.2) SsCallerSource
		 Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsCallerSource[] callerSources
		 = info.getAnnotationsByType(Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsCallerSource.class);
 
		 for(Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid.SsCallerSource callerSource : callerSources) {
			 
			 SsValidation valid;
			 
			 valid=new SsValidation();			 
			 valid.path="caller.source";
			 valid.ssError= SsError.Err_NotAllowed;
			 valid.ssValid = Serialization.Instance.deserializeFromString("{\"type\":\"Equal\",\"value\":\""+callerSource.value()+"\"}", JsonObject.class);
			 validations.add(valid);
			 
			 
			 valid=new SsValidation();			 
			 valid.path="caller.source";
			 valid.ssError= SsError.Err_NotAllowed;
			 valid.ssValid = Serialization.Instance.deserializeFromString("{\"type\":\"Required\"}", JsonObject.class);			 
			 validations.add(valid);		 
		 }			 
		 //endregion
		 
		return validations.toArray(new SsValidation[validations.size()]);     
     }
	 
	 
	 
	 
	 
	 
	 
	 
	// region ValidType

	/**
	 * { "type":"Equal","value":"Logined"}
	 */
	public static final String ValidType_Equal = "Equal";

	/**
	 * { "type":"Regex","value":"^\\d{11}$"}
	 */
	public static final String ValidType_Regex = "Regex";

	/**
	 * { "type":"Required" }
	 */
	public static final String ValidType_Required = "Required";
	// endregion

}
