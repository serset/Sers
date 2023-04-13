using System;
using System.Runtime.CompilerServices;

using Vit.Core.Module.Serialization;

namespace Vit.Extensions.Json_Extensions
{
    public static partial class ObjectSerializeExtensions
    {

        #region (x.1)object <--> String

        #region Serialize

        /// <summary>
        /// 使用Newtonsoft序列化。
        /// value 可为 struct(int bool string 等) 或者 class（模型 Array JObject等）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize(this object value)
        {
            return Json.SerializeToString(value);
        }
        #endregion


        #region Deserialize

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Deserialize(this string value, Type type)
        {
            return Json.DeserializeFromString(value, type);
        }

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(this string value)
        {
            return Json.DeserializeFromString<T>(value);
        }

        #endregion

        #endregion


        #region (x.2)object <--> bytes

        #region SerializeToBytes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeToBytes(this object value)
        {
            return Json.SerializeToBytes(value);
        }
        #endregion

        #region DeserializeFromBytes 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeFromBytes<T>(this byte[] value)
        {
            return Json.DeserializeFromBytes<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DeserializeFromBytes(this byte[] value, Type type)
        {
            return Json.DeserializeFromBytes(value, type);
        }
        #endregion

        #endregion


        #region (x.3)object <--> ArraySegmentByte


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> SerializeToArraySegmentByte(this object value)
        {
            return Json.SerializeToArraySegmentByte(value);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeFromArraySegmentByte<T>(this ArraySegment<byte> value)
        {
            return Json.DeserializeFromArraySegmentByte<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DeserializeFromArraySegmentByte(this ArraySegment<byte> value, Type type)
        {
            return Json.DeserializeFromArraySegmentByte(value, type);
        }

        #endregion






        #region (x.4)ConvertBySerialize


        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ConvertBySerialize(this object value, Type type)
        {
            var str = Json.SerializeToString(value);
            return Json.DeserializeFromString(str, type);
        }

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTarget ConvertBySerialize<TTarget>(this object value)
        {
            var str = Json.SerializeToString(value);
            return Json.DeserializeFromString<TTarget>(str);
        }
        #endregion








    }
}
