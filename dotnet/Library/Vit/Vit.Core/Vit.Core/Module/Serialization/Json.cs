
using System;
using System.Runtime.CompilerServices;

using Vit.Extensions.Json_Extensions;

namespace Vit.Core.Module.Serialization
{
    public static class Json
    {

        public static ISerialization Instance { get; set; } = Serialization_Newtonsoft.Instance;


        #region (x.1)object <--> String

        /// <summary>
        /// T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SerializeToString<T>(T value)
        {
            return Instance.SerializeToString<T>(value);
        }

        /// <summary>
        /// T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SerializeToString(object value, Type type)
        {
            return Instance.SerializeToString(value, type);
        }



        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>    
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeFromString<T>(string value)
        {
            return Instance.DeserializeFromString<T>(value);
        }

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DeserializeFromString(string value, Type type)
        {
            return Instance.DeserializeFromString(value, type);
        }

        #endregion

        #region (x.1)object <--> String

        /// <summary>
        /// T也可为值类型（例如 int?、bool）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize<T>(T value)
        {
            return Instance.SerializeToString<T>(value);
        }

        /// <summary>
        /// T也可为值类型（例如 int?、bool）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize(object value, Type type)
        {
            return Instance.SerializeToString(value, type);
        }



        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(string value)
        {
            return Instance.DeserializeFromString<T>(value);
        }

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Deserialize(string value, Type type)
        {
            return Instance.DeserializeFromString(value, type);
        }

        #endregion


        #region (x.2)object <--> bytes

        /// <summary>
        /// T 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeToBytes<T>(T obj)
        {
            return Instance.SerializeToBytes<T>(obj);
        }

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeToBytes(object value, Type type)
        {
            return Instance.SerializeToBytes(value, type);
        }

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
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
        /// type 可以为   byte[]、string、 object 、struct
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
        /// type 可以为   byte[]、string、 object 、struct
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
        /// type 可以为   byte[]、string、 object 、struct
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



        #region (x.3)object <--> Span

        //T DeserializeFromSpan<T>(ReadOnlyMemory<byte> bytes);

        //object DeserializeFromSpan(ReadOnlyMemory<byte> bytes, Type type);

        #endregion



        #region (x.4)object <--> ArraySegmentByte

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> SerializeToArraySegmentByte(this object value)
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

    }
}
