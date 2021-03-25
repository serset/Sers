using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.ServiceCenter.Entity;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions;

namespace Sers.Gover.RateLimit
{
    public class RateLimitMng
    {

        /// <summary>
        /// rateLimitType -> RateLimitType 限制
        /// </summary>
        [JsonIgnore]
        ConcurrentDictionary<string, Type> limitType_Map = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// rateLimitKey -> IRateLimit 映射
        /// </summary>
        [JsonProperty]
        ConcurrentDictionary<string, IRateLimit> limit_Map = new ConcurrentDictionary<string, IRateLimit>();


        private IRateLimit[] limits=new IRateLimit[0];


        public RateLimitMng()
        {
            LimitType_Add("FixedWindow", typeof(FixedWindow));
        }


        #region 服务限流规则 管理
        public void LimitType_Add(string rateLimitType, Type type)
        {
            limitType_Map[rateLimitType] = type;
        }

        public void LimitType_Remove(string rateLimitType)
        {
            limitType_Map.TryRemove(rateLimitType, out _);
        }
        #endregion



        #region 服务限流项管理
        /// <summary>
        /// 获取所有限流项目
        /// </summary>
        /// <returns></returns>
        public IRateLimit[] RateLimit_GetAll()
        {
            return limits;
        }



        public void RateLimit_Remove(string rateLimitKey)
        {
            if (limit_Map.TryRemove(rateLimitKey, out _))
            {
                limits = limit_Map.Values.ToArray();
            }
        }

        public bool RateLimit_Add(JObject rateLimit)
        {
            if (!limitType_Map.TryGetValue(rateLimit["rateLimitType"].Value<string>(),out var type)) return false;

            var limitItem=rateLimit.Deserialize(type) as IRateLimit;

            if (null == limitItem) return false;

            limit_Map[limitItem.rateLimitKey] = limitItem;

            limits = limit_Map.Values.ToArray();
            return true;
        }

        #endregion


        #region 接口调用时触发的事件


        /// <summary>
        /// 若返回不为null，则对应服务被限流(服务直接返回对应错误)
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SsError BeforeLoadBalancing(RpcContextData rpcData, ApiMessage requestMessage)
        {
            foreach (var rateLimit in limits)
            {
                var error = rateLimit.BeforeLoadBalancing(rpcData, requestMessage);
                if (null != error)
                    return error;
            }
            return null;
        }


        /// <summary>
        /// 若返回不为null，则对应服务被限流(服务直接返回对应错误)
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        /// <param name="apiNode"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SsError BeforeCallRemoteApi(RpcContextData rpcData, ApiMessage requestMessage,ApiNode apiNode)
        {
            foreach (var rateLimit in limits)
            {
                var error = rateLimit.BeforeCallRemoteApi(rpcData, requestMessage, apiNode);
                if (null != error)
                    return error;
            }
            return null;
        }

        
        #endregion

    }
}
