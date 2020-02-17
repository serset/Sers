using Vit.Core.Util.Guid;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Vit.Core.Util.Common
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


        /// <summary>
        /// 获取guid。如："1f3c6041c68f4ab3ae19f66f541e3209"
        /// </summary>
        /// <returns></returns>
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
        /// 返回随机数
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Random(int minValue, int maxValue)
        {
            return new Random(global::System.Guid.NewGuid().GetHashCode()).Next(minValue, maxValue);
        }

        public static long NewGuidLong()
        {
            return Snowflake.GetId();
        }


        /// <summary>
        /// MD5加密字符串（32位大写）
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string result = BitConverter.ToString(md5.ComputeHash(bytes));
            return result.Replace("-", "");
        }

    }
}
