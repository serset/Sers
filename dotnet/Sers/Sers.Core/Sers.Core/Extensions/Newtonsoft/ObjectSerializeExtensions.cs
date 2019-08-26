using Newtonsoft.Json;
using System;

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
            var str = value.Serialize();
            return str.Deserialize(type);
        }

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertBySerialize<T>(this Object value)
        {
            var str = value.Serialize();
            return str.Deserialize<T>();
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
            if (null == value) return null;

            if (value.GetType().TypeIsValueTypeOrStringType())
            {
                return value.Convert<string>();
            }
            return JsonConvert.SerializeObject(value);
        }
        #endregion


        #region Deserialize

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(this string value,Type type)
        {
            if (null == value || null == type) return null;
         
            if (type.TypeIsValueTypeOrStringType())
            {
                return DeserializeStruct(value, type);
            }
            return DeserializeClass(value, type);
        }

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return (T)Deserialize(value,typeof(T));
            //if (typeof(T).IsValueTypeOrStringType())
            //{
            //    return (T)DeserializeStruct<T>(value);
            //}
            //return DeserializeClass<T>(value);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">必须为 where T : struct</typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        private static object DeserializeStruct(string value, Type type)
        {
            try
            {
                if (type.IsStringType())
                    return value;
                return value.Convert(type);
            }
            catch { }
            return type.DefaultValue();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">必须为 where T : struct</typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        private static object DeserializeStruct<T>(string value)
        {
            return DeserializeStruct(value,typeof(T));
            //try
            //{
            //    if (typeof(T).IsStringType())
            //        return value;

            //    return value.Convert<T>();
            //}
            //catch { }
            //return default(T);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"> 必须为 where T : class </param>
        /// <returns></returns>
        private static object DeserializeClass(string value, Type type)
        {
            if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();
            return JsonConvert.DeserializeObject(value, type);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">必须为 where T : class</typeparam>
        /// <param name="value"></param>
        private static T DeserializeClass<T>(string value)
        {
            return (T)DeserializeClass(value, typeof(T));
            //if (string.IsNullOrWhiteSpace(value)) return default(T);
            //return JsonConvert.DeserializeObject<T>(value);
        }


        #endregion

    }
}
