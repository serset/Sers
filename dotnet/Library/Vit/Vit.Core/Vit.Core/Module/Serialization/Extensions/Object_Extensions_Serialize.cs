using System;
using System.Runtime.CompilerServices;

using Vit.Core.Module.Serialization;

namespace Vit.Extensions.Serialize_Extensions
{
    public static partial class Object_Extensions_Serialize
    {

        #region #1 object <--> String

        #region Serialize

        /// <summary>
        /// value type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize(this object value)
        {
            return Json.Serialize(value);
        }
        #endregion


        #region Deserialize

        /// <summary>
        /// Deserialize to type, type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Deserialize(this string value, Type type)
        {
            return Json.Deserialize(value, type);
        }

        /// <summary>
        /// Deserialize to T, T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(this string value)
        {
            return Json.Deserialize<T>(value);
        }

        #endregion

        #endregion


        #region #2 object <--> bytes

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


        #region #3 object <--> ArraySegmentByte


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



    }
}
