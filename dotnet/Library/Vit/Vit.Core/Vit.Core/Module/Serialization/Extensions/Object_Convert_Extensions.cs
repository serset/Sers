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
            return Json.Deserialize(Json.Serialize(value), type);
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
            return Json.Deserialize<TTarget>(Json.Serialize(value));
        }


    }
}
