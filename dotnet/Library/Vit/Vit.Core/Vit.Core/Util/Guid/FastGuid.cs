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

        //  1 byte        1 byte           4 byte                2 byte             
        //  机器ID        数据ID            时间                  自增             
        // machineId    dataCenterId       seconds                curId
        private static long machineId = CommonHelp.Random(0, 127);//机器ID
        private static long dataCenterId = CommonHelp.Random(0, 255);//数据ID
        //private static int seconds;
        private static int curId=0;



        static DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetTimestamp()
        {
            return  (DateTime.UtcNow - start).TotalMilliseconds;
        }

    


        /// <summary>
        /// 获取长整形的ID
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetGuid()
        {
            long seconds = (long)GetTimestamp();
            long id = Interlocked.Increment(ref curId);
            return (((((machineId << 8) | dataCenterId) << 32) | seconds) << 8) | id;
        }

    }
}
