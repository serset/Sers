using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Extensions
{
    public static partial class TypeExtensions
    {

        #region IsStringType

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"></see> is  string type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStringType(this global::System.Type type)
        {
            return type == typeof(string);
        }
        #endregion



        #region TypeIsValueTypeOrStringType

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"></see> is a value type or string type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TypeIsValueTypeOrStringType(this global::System.Type type)
        {
            if (type == null)
            {
                return false;
                //throw new ArgumentNullException(nameof(type));
            }
            return type.IsValueType || type.IsStringType();
        }
        #endregion

       



        #region GetUnderlyingTypeIfNullable

        /// <summary>
        /// 若为Nullable类型（例如 long?）则获取对应的值类型(例如long)，否则返回自身。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static global::System.Type GetUnderlyingTypeIfNullable(this global::System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            //Nullable.GetUnderlyingType(type);

            // We need to check whether the property is NULLABLE
            // If it is NULLABLE, then get the underlying type. eg if "Nullable<long>" then this will return just "long"
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) ? type.GetGenericArguments()[0] : type;
        }
        #endregion


        #region DefaultValue

        /// <summary>
        /// 功能类似 default(T)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultValue(this Type type)
        {
            if (null == type || !type.IsValueType) return null;
            return Activator.CreateInstance(type);
        }
        #endregion

        #region Convert



        /// <summary>
        /// 若Type为Nullable类型（例如 long?）则转换为对应的值类型(例如long)，否则直接转换。
        /// 若转换失败，会返回default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Convert<T>(this Object value)
        {
            if (value == null)
            {
                return default(T);
                //throw new ArgumentNullException(nameof(value));
            }
            //try
            //{
                return (T)System.Convert.ChangeType(value, typeof(T).GetUnderlyingTypeIfNullable());
            //}
            //catch (System.Exception)
            //{

            //    throw;
            //    return default(T);
            //}
            
        }


        /// <summary>
        /// 若为Nullable类型（例如 long?）则转换为对应的值类型(例如long)，否则直接转换。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Object Convert(this Object value, global::System.Type type)
        {
            if (value == null)
            {
                return null;
                //throw new ArgumentNullException(nameof(value));
            }
            return System.Convert.ChangeType(value, type.GetUnderlyingTypeIfNullable());
        }
        #endregion


    }
}
