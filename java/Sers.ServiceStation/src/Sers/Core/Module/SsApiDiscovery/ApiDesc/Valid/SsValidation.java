/**
 * 
 */
package Sers.Core.Module.SsApiDiscovery.ApiDesc.Valid;

import static java.lang.annotation.ElementType.METHOD;
import static java.lang.annotation.RetentionPolicy.RUNTIME;

import java.lang.annotation.Documented;
import java.lang.annotation.Repeatable;
import java.lang.annotation.Retention;
import java.lang.annotation.Target;

@Documented
@Retention(RUNTIME)
@Target(METHOD)
@Repeatable(value = SsValidations.class)
/**
 * demo "fold1/fold2"
 */
public @interface SsValidation {
	
	 
	String path();	

	String errorMessage() default "";
	int errorCode() default -1;	
	
	String ssValid();
	
}
