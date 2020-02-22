using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Vit.Core.Util.Guid
{
    /// <summary>
    /// 动态生产有规律的ID Snowflake算法是Twitter的工程师为实现递增而不重复的ID实现的
    /// http://blog.csdn.net/w200221626/article/details/52064976
    /// C# 实现 Snowflake算法 
    /// </summary>
    public class Snowflake
    {
        private static long machineId=0;//机器ID
        private static long datacenterId = 0L;//数据ID


        private static long sequence = 0L;//计数从零开始
        private static long lastTimestamp = -1L;//最后时间戳

        private static readonly object syncRoot = new object();//加锁对象


        private static long twepoch = 1288834974657L;

        private const long machineIdBits = 5L; //机器码字节数
        private const long datacenterIdBits = 5L;//数据字节数
        private const long maxMachineId = -1L ^ -1L << (int)machineIdBits; //最大机器ID
        private const long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);//最大数据ID


        private const long timestampLeftShift = 22; //时间戳左移动位数就是机器码+计数器总字节数+数据字节数
        private const long datacenterIdShift = 17;
        private const long machineIdShift = 12; //机器码数据左移位数，就是后面计数器占用的位数      


        private const long sequenceMask = -1L ^ -1L << 12; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成





        #region static 
        static Snowflake()
        {
            try
            {
                SetMachineId(CommonHelp.Random(0, 31));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            try
            {
                SetDatacenterId(CommonHelp.Random(0, 31));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }     
        }
        /// <summary>
        /// 5位(0~31)
        /// </summary>
        /// <param name="machineId"></param>
        public static void SetMachineId(long machineId)
        {
            if (machineId >= 0)
            {
                if (machineId <= maxMachineId)
                {
                    Snowflake.machineId = machineId;
                    return;
                }
            }
            throw new Exception("机器码ID非法");
        }

        /// <summary>
        /// 5位(0~31)
        /// </summary>
        /// <param name="datacenterId"></param>
        public static void SetDatacenterId(long datacenterId)
        {            

            if (datacenterId >= 0)
            {
                if (datacenterId <= maxDatacenterId)
                {
                    Snowflake.datacenterId = datacenterId;
                    return;
                }
            }
            throw new Exception("数据中心ID非法");
        }
        #endregion


       

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        private static long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            int count = 0;
            while (timestamp <= lastTimestamp)//这里获取新的时间,可能会有错,这算法与comb一样对机器时间的要求很严格
            {
                count++;
                if (count > 10)
                    throw new Exception("机器的时间可能不对");
                Thread.Sleep(1);
                timestamp = GetTimestamp();
            }
            return timestamp;
        }

        /// <summary>
        /// 获取长整形的ID
        /// </summary>
        /// <returns></returns>
        public static long GetId()
        {
            lock (syncRoot)
            {
                long timestamp = GetTimestamp();
                if (Snowflake.lastTimestamp == timestamp)
                { //同一微妙中生成ID
                    sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (sequence == 0)
                    {
                        //一微妙内产生的ID计数已达上限，等待下一微妙
                        timestamp = GetNextTimestamp(Snowflake.lastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID
                    sequence = 0L;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                Snowflake.lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long Id = ((timestamp - twepoch) << (int)timestampLeftShift)
                    | (datacenterId << (int)datacenterIdShift)
                    | (machineId << (int)machineIdShift)
                    | sequence;
                return Id;
            }
        }

    }
}
