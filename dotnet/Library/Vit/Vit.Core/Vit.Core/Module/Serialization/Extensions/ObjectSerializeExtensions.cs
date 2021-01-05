﻿using System;
using Vit.Core.Module.Serialization;

namespace Vit.Extensions
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
        public static string Serialize(this object value)
        {
            return Serialization.Instance.SerializeToString(value);
        }
        #endregion


        #region Deserialize

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(this string value, Type type)
        {
            return Serialization.Instance.DeserializeFromString(value, type);
        }

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return Serialization.Instance.DeserializeFromString<T>(value);
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
        public static byte[] SerializeToBytes(this object value)
        {
            return Serialization.Instance.SerializeToBytes(value);
        }
        #endregion

        #region DeserializeFromBytes 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(this byte[] value)
        {
            return Serialization.Instance.DeserializeFromBytes<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeFromBytes(this byte[] value, Type type)
        {
            return Serialization.Instance.DeserializeFromBytes(value, type);
        }
        #endregion

        #endregion


        #region (x.3)object <--> ArraySegmentByte

        #region SerializeToArraySegmentByte
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ArraySegment<byte> SerializeToArraySegmentByte(this object value)
        {
            return Serialization.Instance.SerializeToArraySegmentByte(value);
        }
        #endregion


        #region DeserializeFromArraySegmentByte
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeFromArraySegmentByte<T>(this ArraySegment<byte> value)
        {
            return Serialization.Instance.DeserializeFromArraySegmentByte<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeFromArraySegmentByte(this ArraySegment<byte> value, Type type)
        {
            return Serialization.Instance.DeserializeFromArraySegmentByte(value, type);
        }
        #endregion
        #endregion



        #region (x.4)ConvertBySerialize


        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertBySerialize(this object value, Type type)
        {
            return Serialization.Instance.ConvertBySerialize(value, type);
        }

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertBySerialize<T>(this object value)
        {
            return Serialization.Instance.ConvertBySerialize<T>(value);
        }
        #endregion


       





    }
}