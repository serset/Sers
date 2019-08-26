using System;
using Sers.Core.Module.Serialization;

namespace Sers.Core.Extensions
{
    public static partial class ObjectSerializeExtensions
    {

        #region ConvertBySerialize


        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertBySerialize(this Object value, Type type)
        {
            return Serialization.Instance.ConvertBySerialize(value, type);
        }

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertBySerialize<T>(this Object value)
        {
            return Serialization.Instance.ConvertBySerialize<T>(value);
        }
        #endregion


        #region Serialize

 

        /// <summary>
        /// 使用Newtonsoft序列化。
        /// value 可为 struct(int bool string 等) 或者 class（模型 Array JObject等）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(this Object value)
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
            return Serialization.Instance.DeserializeFromString(value,type);
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


        #region SerializeToBytes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SerializeToBytes(this Object value)
        {
            return Serialization.Instance.Serialize(value);
        }
        #endregion


        #region DeserializeFromBytes

        public static T DeserializeFromBytes<T>(this ArraySegment<byte> value)
        {
            return  Serialization.Instance.Deserialize<T>(value);
        }
        #endregion
        
    }
}
