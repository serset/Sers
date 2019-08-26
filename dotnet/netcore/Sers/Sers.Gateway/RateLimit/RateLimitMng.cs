using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Util.SsError;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Sers.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Sers.Core.Util.ConfigurationManager;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sers.Core.Module.Log;

namespace Sers.Gateway.RateLimit
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

        /// <summary>
        /// 从配置文件加载 服务限流配置
        /// </summary>
        public void LoadFromConfiguration()
        {
            var rateLimits = ConfigurationManager.Instance.GetByPath<List<JObject>>("Sers.Gateway.rateLimit");
            if (null != rateLimits)
            {
                foreach (var item in rateLimits)
                {
                    RateLimit_Add(item);
                }
            }
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

            Logger.Info("[服务限流]加载成功。config:"+ rateLimit);

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
        public SsError BeforeCall(HttpContext context)
        {
            foreach (var rateLimit in limits)
            {
                var error = rateLimit.BeforeCall(context);
                if (null != error)
                    return error;
            }
            return null;
        }


        

        public void OnFinally(HttpContext context)
        {
            foreach (var rateLimit in limits)
            {
                rateLimit.OnFinally(context);
            }
        }
        #endregion

    }
}
