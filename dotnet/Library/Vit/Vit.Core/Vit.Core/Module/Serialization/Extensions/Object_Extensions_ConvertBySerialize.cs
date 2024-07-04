using System;
using System.Runtime.CompilerServices;

using Vit.Core.Module.Serialization;

namespace Vit.Extensions.Serialize_Extensions
{
    public static partial class Object_Extensions_ConvertBySerialize
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
            if (value is string str)
            {
                if (type == typeof(string)) return str;
                return Json.Deserialize(str, type);
            }

            str = Json.Serialize(value);
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
            return (TTarget)ConvertBySerialize(value, typeof(TTarget));
        }
    }
}
