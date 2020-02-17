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
 * 标识函数第一个参数为Api的参数实体。（忽略其他参数(若存在)，调用函数时其他参数(若存在)为空）
 * @author lith
 *
 */
public @interface SsArgEntity {
 
}
