using System;

namespace Sers.Core.Extensions
{
    public static partial class EnumExtensions
    {




        #region Enum <--> String

        /// <summary>
        /// T 必须为Enum
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

  


        public static string EnumToString(this Enum data)
        {
            try
            {
                return Enum.GetName(data.GetType(), data);
            }
            catch { }
            return null;
        }
        #endregion



        public static bool ValueIsDefined(this Enum data, Object value)
        {
            return Enum.IsDefined(data.GetType(), value);
        }



    }
}
