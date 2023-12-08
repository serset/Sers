using System.Data;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static partial class IDataReader_ReadValue_Extensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDateTime(this IDataReader dr, int i)
        {
            var typeName = dr.GetDataTypeName(i);
            return typeName == "DATE" || typeName == "TIME" || typeName == "DATETIME" || typeName == "DATETIME2";

            //return dr.GetDataTypeName(i) == "DATETIME";
            //return dr.GetDataTypeName(i).ToLower().Contains("datetime");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <param name="DateTimeFormat">时间序列化格式</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize(this IDataReader dr, int i, string DateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }

            if (IsDateTime(dr, i))
            {
                return dr.GetDateTime(i).ToString(DateTimeFormat);
            }
            return dr[i].ToString();
        }


    }
}
