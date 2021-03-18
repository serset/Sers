using Vit.Core.Util.Common;
using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Vit.Core.Util.Guid
{

    public class FastGuid
    {
        // 64 bit
        // 8 byte   


        private static long machineId= CommonHelp.Random(0, 127);//机器ID
        private static long datacenterId = CommonHelp.Random(0, 255);//数据ID
         

         
       

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        private static long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private static long curGuid;
        static FastGuid() 
        {
            curGuid = ((machineId << 8) | datacenterId) << 48;
            curGuid = curGuid | GetTimestamp();
        }



        /// <summary>
        /// 获取长整形的ID
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetGuid()
        {
            return Interlocked.Increment(ref curGuid);             
        }

    }
}
