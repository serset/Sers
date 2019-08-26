using Microsoft.AspNetCore.Http;
using Sers.Core.Util.SsError;


namespace Sers.Gateway.RateLimit
{
    public interface IRateLimit
    {
        string rateLimitKey { get; }

        string rateLimitType { get; }


        /// <summary>
        /// 若返回不为null，则对应服务被限流(服务直接返回对应错误)
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        SsError BeforeCall(HttpContext context);


       

        void OnFinally(HttpContext context);
    }
}
