using System;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Serialize_Extensions
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
        public static bool IsStringType(this Type type)
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
        public static bool TypeIsValueTypeOrStringType(this Type type)
        {
            if (type == null)
            {
                return false;
                //throw new ArgumentNullException(nameof(type));
            }
            return type.IsValueType || type.IsStringType();
        }
        #endregion



        #region IsNullable
        /// <summary>
        /// 是否为Nullable类型（例如 long?）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullable(this Type type)
        {
            return true == type?.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion

        #region GetUnderlyingTypeIfNullable

        /// <summary>
        /// If it is NULLABLE, then get the underlying type. eg if "Nullable&lt;long&gt;" then this will return just "long"
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetUnderlyingTypeIfNullable(this Type type)
        {
            //return type.IsNullable() ? type.GetGenericArguments()[0] : type;
            return type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;


            //if (type == null)
            //{
            //    throw new ArgumentNullException(nameof(type));
            //}

            ////Nullable.GetUnderlyingType(type);

            //// We need to check whether the property is NULLABLE
            //return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) ? type.GetGenericArguments()[0] : type;
        }
        #endregion


        #region IsNumericType
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            type = GetUnderlyingTypeIfNullable(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
            }
            return false;
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
