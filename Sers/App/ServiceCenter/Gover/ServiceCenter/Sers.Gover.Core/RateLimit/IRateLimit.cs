using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Rpc;
using Sers.Core.Util.SsError;
using Sers.ServiceCenter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Gover.Core.RateLimit
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
        SsError BeforeLoadBalancing(IRpcContextData rpcData, ApiMessage requestMessage);


        /// <summary>
        /// 若返回不为null，则对应服务被限流(服务直接返回对应错误)
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        /// <param name="apiNode"></param>
        /// <returns></returns>
        SsError BeforeCallRemoteApi(IRpcContextData rpcData, ApiMessage requestMessage, ApiNode apiNode);

        void OnFinally(IRpcContextData rpcData, ApiMessage requestMessage);
    }
}
