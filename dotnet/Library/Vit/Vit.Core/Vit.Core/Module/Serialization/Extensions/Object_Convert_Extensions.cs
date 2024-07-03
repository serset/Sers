using System;
using System.Runtime.CompilerServices;

using Vit.Core.Module.Serialization;

namespace Vit.Extensions.Json_Extensions
{
    public static partial class Object_Convert_Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ConvertBySerialize(this object value, Type type)
        {
            string str;
            if (value is string strValue) str = strValue;
            else str = Json.Serialize(value);

            return Json.Deserialize(str, type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTarget ConvertBySerialize<TTarget>(this object value)
        {
            string str;
            if (value is string strValue) str = strValue;
            else str = Json.Serialize(value);

            return Json.Deserialize<TTarget>(str);
        }


    }
}
