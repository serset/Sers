/**
 * 
 */
package Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation;

import static java.lang.annotation.ElementType.METHOD;
import static java.lang.annotation.RetentionPolicy.RUNTIME;

import java.lang.annotation.Documented;
import java.lang.annotation.Repeatable;
import java.lang.annotation.Retention;
import java.lang.annotation.Target;

@Documented
@Retention(RUNTIME)
@Target(METHOD)
@Repeatable(value = SsRoutes.class)
/**
 * demo "fold1/fold2"
 *
 */
public @interface SsRoute {
	
	/**
	 * demo "fold1/fold2"
	 * @return
	 */	
	String value();
}
