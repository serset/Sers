using System;

namespace Vit.Extensions
{
    public static class TimeStamp_Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccurateToMilliseconds">是否精确到毫秒</param>
        /// <returns>返回一个长整数时间戳</returns>
        public static long ToTimeStamp(this DateTime time,bool AccurateToMilliseconds = true)
        {
            if (AccurateToMilliseconds)
            {
                // 使用当前时间计时周期数（636662920472315179）减去1970年01月01日计时周期数（621355968000000000）除去（删掉）后面4位计数（后四位计时单位小于毫秒，快到不要不要）再取整（去小数点）。
                //备注：DateTime.Now.ToUniversalTime不能缩写成DateTime.Now.Ticks，会有好几个小时的误差。
                //621355968000000000计算方法 long ticks = (new DateTime(1970, 1, 1, 8, 0, 0)).ToUniversalTime().Ticks;
                return (time.Ticks - 621355968000000000) / 10000;
            }
            else
            {
                //上面是精确到毫秒，需要在最后除去（10000），这里只精确到秒，只要在10000后面加三个0即可（1秒等于1000毫米）。
                return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }
        }


       
        /// <summary>
        /// 时间戳转换为时间
        /// </summary>
        /// <param name="TimeStamp">时间戳</param>
        /// <param name="AccurateToMilliseconds">是否精确到毫秒</param>
        /// <returns>返回一个日期时间</returns>
        public static DateTime TimeStampToDateTime(this long TimeStamp, bool AccurateToMilliseconds = true)
        {           
            if (AccurateToMilliseconds)
            {
                return startTime.AddTicks(TimeStamp * 10000);
            }
            else
            {
                return startTime.AddTicks(TimeStamp * 10000000);
            }
        }
        /// <summary>
        /// 时间戳的起始时间
        /// </summary>
        public static System.DateTime startTime = new System.DateTime(1970, 1, 1);


        /// <summary>
        /// 时间戳转换为时间(使用本地时区)
        /// </summary>
        /// <param name="TimeStamp">时间戳</param>
        /// <param name="AccurateToMilliseconds">是否精确到毫秒</param>
        /// <returns>返回一个日期时间</returns>
        public static DateTime TimeStampToLocalDateTime(this long TimeStamp, bool AccurateToMilliseconds = true)
        {
            if (AccurateToMilliseconds)
            {
                return startTimeLocal.AddTicks(TimeStamp * 10000);
            }
            else
            {
                return startTimeLocal.AddTicks(TimeStamp * 10000000);
            }
        }
        /// <summary>
        /// 时间戳的起始时间(使用本地时区)
        /// </summary>
        public static System.DateTime startTimeLocal = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
    }
}
