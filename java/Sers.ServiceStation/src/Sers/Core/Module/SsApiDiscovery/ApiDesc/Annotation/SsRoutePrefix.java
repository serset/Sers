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
 *  路由前缀,例如："demo/v1"
 *
 */
public @interface SsRoutePrefix {
	
	/**
	 *  路由前缀,例如："demo/v1"
	 * @return
	 */	
	String value();
}
