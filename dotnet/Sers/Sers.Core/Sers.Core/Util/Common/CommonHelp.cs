using Sers.Core.Util.Guid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sers.Core.Util.Common
{
    public static class CommonHelp
    {
        /// <summary>
        /// 获取相对路径。如 ["Data","Sers","Gover", "Counter.json"],将返回 /root/netapp/xxxx/Data/Sers/Gover/Counter.json
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetAbsPathByRealativePath(params string[] path)
        {
            return Path.Combine(AppContext.BaseDirectory , String.Join( Path.DirectorySeparatorChar.ToString(), path));
        }


        public static string NewGuid()
        {
            return global::System.Guid.NewGuid().ToString("N");
            /*
var guid = Guid.NewGuid();
Debug.WriteLine(guid.ToString());   //1f3c6041-c68f-4ab3-ae19-f66f541e3209
Debug.WriteLine(guid.ToString("N"));//1f3c6041c68f4ab3ae19f66f541e3209
Debug.WriteLine(guid.ToString("D"));//1f3c6041-c68f-4ab3-ae19-f66f541e3209
Debug.WriteLine(guid.ToString("B"));//{1f3c6041-c68f-4ab3-ae19-f66f541e3209}
Debug.WriteLine(guid.ToString("P"));//(1f3c6041-c68f-4ab3-ae19-f66f541e3209)
Debug.WriteLine(guid.ToString("X"));//{0x1f3c6041,0xc68f,0x4ab3,{0xae,0x19,0xf6,0x6f,0x54,0x1e,0x32,0x09}}
             
             
             */
        }


        public static int Random(int minValue, int maxValue)
        {
            return new Random(global::System.Guid.NewGuid().GetHashCode()).Next(minValue, maxValue);
        }

        public static long NewGuidLong()
        {
            return Snowflake.GetId();
        }

       


    }
}
