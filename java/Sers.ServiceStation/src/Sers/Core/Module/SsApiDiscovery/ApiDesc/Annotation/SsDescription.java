/**
 * 
 */
package Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation;

import static java.lang.annotation.ElementType.METHOD;
import static java.lang.annotation.RetentionPolicy.RUNTIME;

import java.lang.annotation.Documented;
import java.lang.annotation.Retention;
import java.lang.annotation.Target;

@Documented
@Retention(RUNTIME)
/**
 * @author help
 *
 */
public @interface SsDescription {
	
	/**
	  *  描述
	 * @return
	 */	
	String value();
}
