using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

using Vit.Core.Util.Guid;
using Vit.Extensions.Serialize_Extensions;

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
            return Path.Combine(AppContext.BaseDirectory, Path.Combine(path));
        }


        /// <summary>
        /// 返回随机数。 the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(int minValue, int maxValue)
        {
            return new Random(global::System.Guid.NewGuid().GetHashCode()).Next(minValue, maxValue);
        }


        #region NewGuid    

        /// <summary>
        /// 通过系统Guid对象生成guid。如："1f3c6041c68f4ab3ae19f66f541e3209"
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// 通过系统Guid对象生成guid
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NewGuidLong()
        {
            byte[] buffer = global::System.Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
        #endregion



        #region Snowflake       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NewSnowflakeGuidLong()
        {
            return Snowflake.GetId();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NewSnowflakeGuid()
        {
            return NewSnowflakeGuidLong().Int64ToHexString();
        }
        #endregion

        #region FastGuid       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NewFastGuidLong()
        {
            return FastGuid.GetGuid();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NewFastGuid()
        {
            return NewFastGuidLong().Int64ToHexString();
        }
        #endregion







        /// <summary>
        /// MD5加密字符串（32位大写）
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字符串</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string MD5(string source)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(source);
                string result = BitConverter.ToString(md5.ComputeHash(bytes));
                return result.Replace("-", "");
            }
        }

        /// <summary>
        /// MD5加密（32位大写）
        /// </summary>
        /// <param name="bytes">源数据</param>
        /// <returns>加密后的字符串</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string MD5(byte[] bytes)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string result = BitConverter.ToString(md5.ComputeHash(bytes));
                return result.Replace("-", "");
            }
        }

    }
}
