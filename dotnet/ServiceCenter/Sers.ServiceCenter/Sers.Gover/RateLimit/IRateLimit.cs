using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.ServiceCenter.Entity;
using Vit.Core.Util.ComponentModel.SsError;

namespace Sers.Gover.RateLimit
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
        SsError BeforeLoadBalancing(RpcContextData rpcData, ApiMessage requestMessage);


        /// <summary>
        /// 若返回不为null，则对应服务被限流(服务直接返回对应错误)
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        /// <param name="apiNode"></param>
        /// <returns></returns>
        SsError BeforeCallRemoteApi(RpcContextData rpcData, ApiMessage requestMessage, ApiNode apiNode);

 
    }
}
