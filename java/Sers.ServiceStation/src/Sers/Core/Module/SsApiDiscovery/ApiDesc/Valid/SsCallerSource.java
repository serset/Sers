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
/**
 * 
 */
public @interface SsCallerSource {
	
	 
	String value();	
	
	/**
	 * 内部调用
	 */
	public static String Internal="Internal";
	/**
	 * 外部调用(通过网关调用)
	 */
	public static String Outside="Outside";
	
}
