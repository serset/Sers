namespace Vit.Core.Util.ComponentModel.SsError
{
    public partial class SsError
    {


        /// <summary>
        /// 401 权限限制(没有权限)( 401 - Unauthorized 访问被拒绝)
        /// </summary>
        public static readonly SsError Err_NotAllowed = new SsError { errorCode = 401, errorMessage = "权限限制(没有相应权限)" };

        /// <summary>
        /// 404 请求的api不存在
        /// </summary>
        public static readonly SsError Err_ApiNotExists = new SsError { errorCode = 404, errorMessage = "请求的api不存在" };

        /// <summary>
        /// 404 404 Not Found：请求资源不存在
        /// </summary>
        public static readonly SsError Err_404 = new SsError { errorCode = 404, errorMessage = "404 Not Found：请求资源不存在" };

        /// <summary>
        /// 405 请求参数不合法(405 - Method Not Allowed)
        /// </summary>
        public static readonly SsError Err_InvalidParam = new SsError { errorCode = 405, errorMessage = "请求参数不合法" };

        /// <summary>
        /// 408 请求超时
        /// </summary>
        public static readonly SsError Err_Timeout = new SsError { errorCode = 408, errorMessage = "请求超时" };

        /// <summary>
        /// 408 操作超时被强制中断
        /// </summary>
        public static readonly SsError Err_HandleTimeout = new SsError { errorCode = 408, errorMessage = "操作超时被强制中断" };


        /// <summary>
        /// 500 系统出错( 500 - Internal Server Error 服务器遇到了意料不到的情况，不能完成客户的请求。) 
        /// </summary>
        public static readonly SsError Err_SysErr = new SsError { errorCode = 500, errorMessage = "系统出错" };

        /// <summary>
        /// 503 服务限流限制( 503 - Service Unavailable 服务不可用，服务器由于维护或者负载过重未能应答)
        /// </summary>
        public static readonly SsError Err_RateLimit_Refuse = new SsError { errorCode = 503, errorMessage = "服务限流限制" };


        /*
         //StatusCode 参考 http://blog.sina.com.cn/s/blog_6998d7bd01017zl2.html
         
         4xx - 客户端错误
发生错误，客户端似乎有问题。例如，客户端请求不存在的页面，客户端未提供有效的身份验证信息。
· 400 - Bad Request 请求出现语法错误。 
· 401 - Unauthorized 访问被拒绝，客户试图未经授权访问受密码保护的页面。应答中会包含一个WWW-Authenticate头，浏览器据此显示用户名字/密码对话框，然后在 填写合适的Authorization头后再次发出请求。IIS 定义了许多不同的 401 错误，它们指明更为具体的错误原因。这些具体的错误代码在浏览器中显示，但不在 IIS 日志中显示：
    · 401.1 - 登录失败。
    · 401.2 - 服务器配置导致登录失败。
    · 401.3 - 由于 ACL 对资源的限制而未获得授权。
    · 401.4 - 筛选器授权失败。
    · 401.5 - ISAPI/CGI 应用程序授权失败。
    · 401.7 – 访问被 Web 服务器上的 URL 授权策略拒绝。这个错误代码为 IIS 6.0 所专用。
· 403 - Forbidden 资源不可用。服务器理解客户的请求，但拒绝处理它。通常由于服务器上文件或目录的权限设置导致。禁止访问：IIS 定义了许多不同的 403 错误，它们指明更为具体的错误原因：
    · 403.1 - 执行访问被禁止。
    · 403.2 - 读访问被禁止。
    · 403.3 - 写访问被禁止。
    · 403.4 - 要求 SSL。
    · 403.5 - 要求 SSL 128。
    · 403.6 - IP 地址被拒绝。
    · 403.7 - 要求客户端证书。
    · 403.8 - 站点访问被拒绝。
    · 403.9 - 用户数过多。
    · 403.10 - 配置无效。
    · 403.11 - 密码更改。
    · 403.12 - 拒绝访问映射表。
    · 403.13 - 客户端证书被吊销。
    · 403.14 - 拒绝目录列表。
    · 403.15 - 超出客户端访问许可。
    · 403.16 - 客户端证书不受信任或无效。
    · 403.17 - 客户端证书已过期或尚未生效。
    · 403.18 - 在当前的应用程序池中不能执行所请求的 URL。这个错误代码为 IIS 6.0 所专用。
    · 403.19 - 不能为这个应用程序池中的客户端执行 CGI。这个错误代码为 IIS 6.0 所专用。
    · 403.20 - Passport 登录失败。这个错误代码为 IIS 6.0 所专用。
· 404 - Not Found 无法找到指定位置的资源。这也是一个常用的应答。 
    · 404.0 -（无） – 没有找到文件或目录。
    · 404.1 - 无法在所请求的端口上访问 Web 站点。
    · 404.2 - Web 服务扩展锁定策略阻止本请求。
    · 404.3 - MIME 映射策略阻止本请求。
· 405 - Method Not Allowed 请求方法（GET、POST、HEAD、DELETE、PUT、TRACE等）对指定的资源不适用，用来访问本页面的 HTTP 谓词不被允许（方法不被允许）（HTTP 1.1
新） 
· 406 - Not Acceptable 指定的资源已经找到，但它的MIME类型和客户在Accpet头中所指定的不兼容，客户端浏览器不接受所请求页面的 MIME 类型（HTTP 1.1新）。 
· 407 - Proxy Authentication Required 要求进行代理身份验证，类似于401，表示客户必须先经过代理服务器的授权。（HTTP 1.1新） 
· 408 - Request Timeout 在服务器许可的等待时间内，客户一直没有发出任何请求。客户可以在以后重复同一请求。（HTTP 1.1新）
· 409 - Conflict 通常和PUT请求有关。由于请求和资源的当前状态相冲突，因此请求不能成功。（HTTP 1.1新） 
· 410 - Gone 所请求的文档已经不再可用，而且服务器不知道应该重定向到哪一个地址。它和404的不同在于，返回407表示文档永久地离开了指定的位置，而404表示由于未知的
原因文档不可用。（HTTP 1.1新） 
· 411 - Length Required 服务器不能处理请求，除非客户发送一个Content-Length头。（HTTP 1.1新） 
· 412 - Precondition Failed 请求头中指定的一些前提条件失败（HTTP 1.1新）。
· 413 – Request Entity Too Large 目标文档的大小超过服务器当前愿意处理的大小。如果服务器认为自己能够稍后再处理该请求，则应该提供一个Retry-After头（HTTP 1.1
新）。 
· 414 - Request URI Too Long URI太长（HTTP 1.1新）。 
· 415 – 不支持的媒体类型。
· 416 – Requested Range Not Satisfiable 服务器不能满足客户在请求中指定的Range头。（HTTP 1.1新） · 417 – 执行失败。
· 423 – 锁定的错误。


5xx - 服务器错误
服务器由于遇到错误而不能完成该请求。
· 500 - Internal Server Error 服务器遇到了意料不到的情况，不能完成客户的请求。 
    · 500.12 - 应用程序正忙于在 Web 服务器上重新启动。
    · 500.13 - Web 服务器太忙。
    · 500.15 - 不允许直接请求 Global.asa。
    · 500.16 – UNC 授权凭据不正确。这个错误代码为 IIS 6.0 所专用。
    · 500.18 – URL 授权存储不能打开。这个错误代码为 IIS 6.0 所专用。
    · 500.100 - 内部 ASP 错误。
· 501 - Not Implemented 服务器不支持实现请求所需要的功能，页眉值指定了未实现的配置。例如，客户发出了一个服务器不支持的PUT请求。
· 502 - Bad Gateway 服务器作为网关或者代理时，为了完成请求访问下一个服务器，但该服务器返回了非法的应答。 亦说Web 服务器用作网关或代理服务器时收到了无效响应
    · 502.1 - CGI 应用程序超时。
    · 502.2 - CGI 应用程序出错。
· 503 - Service Unavailable 服务不可用，服务器由于维护或者负载过重未能应答。例如，Servlet可能在数据库连接池已满的情况下返回503。服务器返回503时可以提供一个Retry-After头。这个错误代码为 IIS 6.0 所专用。
· 504 - Gateway Timeout 网关超时，由作为代理或网关的服务器使用，表示不能及时地从远程服务器获得应答。（HTTP 1.1新） 。
· 505 - HTTP Version Not Supported 服务器不支持请求中所指明的HTTP版本。（HTTP 1.1新）
 
         
         */



    }
}
