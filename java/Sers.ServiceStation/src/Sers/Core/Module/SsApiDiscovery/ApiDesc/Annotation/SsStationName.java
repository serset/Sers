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
 * 站点名称。例如："SessCenter"
 *
 */
public @interface SsStationName {
	
	/**
	 *  站点名称。例如："SessCenter"
	 * @return
	 */	
	String value();
}
