using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vit.Extensions
{
    public static partial class TypeExtensions
    {

        #region IsStringType

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"></see> is  string type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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



        #region IfNullable
        /// <summary>
        /// 是否为Nullable类型（例如 long?）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IfNullable(this Type type)
        {
            return true== type?.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion

        #region GetUnderlyingTypeIfNullable

        /// <summary>
        /// 若为Nullable类型（例如 long?）则获取对应的值类型(例如long)，否则返回自身。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::System.Type GetUnderlyingTypeIfNullable(this global::System.Type type)
        {
            return type.IfNullable() ? type.GetGenericArguments()[0] : type;


            //if (type == null)
            //{
            //    throw new ArgumentNullException(nameof(type));
            //}

            ////Nullable.GetUnderlyingType(type);

            //// We need to check whether the property is NULLABLE
            //// If it is NULLABLE, then get the underlying type. eg if "Nullable<long>" then this will return just "long"
            //return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) ? type.GetGenericArguments()[0] : type;
        }
        #endregion


        #region DefaultValue

        /// <summary>
        /// 功能类似 default(T)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DefaultValue(this Type type)
        {
            if (null == type || !type.IsValueType) return null;
            return Activator.CreateInstance(type);
        }
        #endregion

       


    }
}
