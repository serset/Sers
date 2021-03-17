using System;

namespace Vit.Extensions
{
    public static partial class EnumExtensions
    {




        #region String --> Enum

        /// <summary>
        /// T 必须为Enum,且不可为Nullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T StringToEnum<T>(this string data)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), data);
            }
            catch { }
            return default(T);
        }




        //public static string EnumToString(this Enum data)
        //{
        //    return data.ToString();
        //    try
        //    {
        //        return Enum.GetName(data.GetType(), data);
        //    }
        //    catch { }
        //    return null;
        //}
        #endregion


        #region ValueIsDefined        
        public static bool ValueIsDefined(this Enum data, object value)
        {
            return Enum.IsDefined(data.GetType(), value);
        }
        #endregion


    }
}
