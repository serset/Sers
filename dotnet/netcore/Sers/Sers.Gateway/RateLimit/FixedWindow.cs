using Microsoft.AspNetCore.Http;
using Sers.Core.Util.SsError;
using System;
using System.Threading;


namespace Sers.Gateway.RateLimit
{



    /// <summary>
    /// 固定时间窗口限流
    /// </summary>
    public class FixedWindow: IRateLimit
    {

        /// <summary>
        /// 限流规则名称,一般对应一个类
        /// </summary>
        public string rateLimitType { get; set; }

        /// <summary>
        /// 限流项名称，必须唯一
        /// </summary>
        public string rateLimitKey { get; set; }

    

        /// <summary>
        /// 时间窗口内最大请求数
        /// </summary>
        public int reqLimit = 1000;
        /// <summary>
        /// 时间窗口ms
        /// </summary>
        public long msInterval = 1000;

        public SsError error = SsError.Err_RateLimit_Refuse;





        private int reqCount = 0;
        private long timeStampStart=0;
        private long timeStampEnd=0;       



        public SsError BeforeCall(HttpContext context)
        {
            
            //TODO: don't use lock !!!
            lock (this)
            {
                DateTime dtNow = DateTime.Now;
                long now = dtNow.Ticks;

                if (now < timeStampEnd)
                {
                    // 在时间窗口内


                    // 判断当前时间窗口内是否超过最大请求控制数
                    if (Interlocked.Increment(ref reqCount) > reqLimit)
                        return error;
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    timeStampStart = now;
                    //timeStampEnd = timeStampStart + msInterval;
                    timeStampEnd = dtNow.AddMilliseconds(msInterval).Ticks;

                    // 超时后重置
                    reqCount = 1;
                    return null;
                }
            }
        }
 
        public void OnFinally(HttpContext context)
        {            
        }
    }
}
