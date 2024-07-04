using System;
using System.Runtime.CompilerServices;

using Vit.Extensions.Serialize_Extensions;

namespace Vit.Core.Module.Serialization
{
    public static partial class Json
    {

        public static ISerialization Instance { get; set; } = Serialization_Newtonsoft.Instance;




        #region #1 object <--> String
        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get "null" if value is null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize(object value, Type type = null)
        {
            return Instance.Serialize(value, type);
        }

        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get "null" if value is null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize<T>(T value)
        {
            return Instance.Serialize(value, typeof(T));
        }



        /// <summary>
        /// Deserialize to T, T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get default value of T if value is null or whiteSpace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(string value)
        {
            return Instance.Deserialize<T>(value);
        }

        /// <summary>
        /// Deserialize to type, type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get default value of T if value is null or whiteSpace
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Deserialize(string value, Type type)
        {
            return Instance.Deserialize(value, type);
        }

        #endregion


        #region #2 object <--> bytes

        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeToBytes(object value, Type type = null)
        {
            return Instance.SerializeToBytes(value, type);
        }

        /// <summary>
        /// T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeFromBytes<T>(byte[] bytes)
        {
            return Instance.DeserializeFromBytes<T>(bytes);
        }

        /// <summary>
        /// type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DeserializeFromBytes(byte[] bytes, Type type)
        {
            return Instance.DeserializeFromBytes(bytes, type);
        }

        /// <summary>
        /// type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(byte[] bytes)
        {
            return Instance.DeserializeFromBytes<T>(bytes);
        }

        /// <summary>
        /// type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Deserialize(byte[] bytes, Type type)
        {
            return Instance.DeserializeFromBytes(bytes, type);
        }

        #endregion




        #region #3 object <--> ArraySegmentByte

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> SerializeToArraySegmentByte(object value)
        {
            return Instance.SerializeToBytes(value).BytesToArraySegmentByte();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
        {
            return Instance.DeserializeFromArraySegmentByte<T>(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type)
        {
            return Instance.DeserializeFromArraySegmentByte(bytes, type);
        }

        #endregion


        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTarget ConvertBySerialize<TTarget>(object value, Type valueType = null)
        {
            return Deserialize<TTarget>(Serialize(value, valueType));
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTarget ConvertBySerialize<TValue, TTarget>(TValue value)
        {
            return ConvertBySerialize<TTarget>(value, typeof(TValue));
        }
    }
}
