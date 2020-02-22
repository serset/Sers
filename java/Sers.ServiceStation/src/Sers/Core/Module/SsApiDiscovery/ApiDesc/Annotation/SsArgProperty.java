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
 * 标识函数第一个参数为Api的参数实体的属性之一。（一般放置在第一个参数上，代表第一个参数不为参数实体）
 * @author lith
 *
 */
public @interface SsArgProperty {
 
}
