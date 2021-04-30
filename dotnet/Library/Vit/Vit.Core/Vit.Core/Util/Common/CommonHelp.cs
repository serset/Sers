using Vit.Core.Util.Guid;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using Vit.Extensions;

namespace Vit.Core.Util.Common
{
    public static class CommonHelp
    {
        /// <summary>            
        /// <para> 构建绝对路径。                                                                                       </para> 
        /// <para> path可为相对路径或绝对路径，若为绝对路径则忽略程序当前路径。                                         </para>
        /// <para> demo: ["Data","Sers","Gover", "Counter.json"],将返回 /root/netapp/xxxx/Data/Sers/Gover/Counter.json  </para>
        /// <para>   ["/Data","Sers","Gover", "Counter.json"],将返回 /Data/Sers/Gover/Counter.json                      </para>
        /// <para>   ["D:\Program Files\Counter.json"],将返回 "D:\Program Files\Counter.json"                           </para>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string GetAbsPath(params string[] path)
        {
            if (path == null || path.Length == 0) return AppContext.BaseDirectory;
            return Path.Combine(AppContext.BaseDirectory , String.Join( Path.DirectorySeparatorChar.ToString(), path));
        }


        /// <summary>
        /// 获取guid。如："1f3c6041c68f4ab3ae19f66f541e3209"
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NewGuid()
        {
            return NewGuidLong().Int64ToHexString();
            //return NewGuidLong().ToString();
            //return global::System.Guid.NewGuid().ToString("N");
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


        /// <summary>
        /// 返回随机数
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(int minValue, int maxValue)
        {
            return new Random(global::System.Guid.NewGuid().GetHashCode()).Next(minValue, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NewGuidLong()
        {
            //return Snowflake.GetId();
            return FastGuid.GetGuid();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NewSnowflakeGuidLong()
        {
            return Snowflake.GetId();           
        }


        /// <summary>
        /// MD5加密字符串（32位大写）
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字符串</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string MD5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string result = BitConverter.ToString(md5.ComputeHash(bytes));
            return result.Replace("-", "");
        }

    }
}
