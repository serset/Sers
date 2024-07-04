using System;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Serialize_Extensions
{
    public static partial class EnumExtensions
    {




        #region String --> Enum

        /// <summary>
        /// enumType must be Enum, can be Nullable
        /// </summary>
        /// <param name="data"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object StringToEnum(this string data, Type enumType)
        {
            try
            {
                return Enum.Parse(enumType.GetUnderlyingTypeIfNullable(), data);
            }
            catch { }
            return default;
        }

        /// <summary>
        /// T must be Enum, can be Nullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T StringToEnum<T>(this string data)
        {
            try
            {
                return (T)Enum.Parse(typeof(T).GetUnderlyingTypeIfNullable(), data);
            }
            catch { }
            return default;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValueIsDefined(this Enum data, object value)
        {
            return Enum.IsDefined(data.GetType(), value);
        }
        #endregion


    }
}
