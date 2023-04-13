using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Object_Extensions
{
    public static partial class Object_Clone_Extensions
    {


        #region CloneProrertyFrom
        /// <summary>
        /// 通过反射复制对象的同名属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CloneProrertyFrom<T>(this T data, T res)
        {
            if (data == null || res == null)
            {
                return data;
            }

            //(x.1)
            Type type = data.GetType();


            //(x.2)
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                if (!info.CanRead || !info.CanWrite) continue;
                object value = info.GetValue(res);
                //if (value == null) continue;
                info.SetValue(data, value);
            }

            //(x.3)
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in fields)
            {
                object value = info.GetValue(res);
                //if (value == null) continue;
                info.SetValue(data, value);
            }

            return data;
        }
        #endregion


        #region CloneNotNullProrertyFrom
        /// <summary>
        /// 通过反射复制对象的非空同名属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CloneNotNullProrertyFrom<T>(this T data, T res)
        {
            if (data == null || res == null)
            {
                return data;
            }

            //(x.1)
            Type type = data.GetType();


            //(x.2)
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                if (!info.CanRead || !info.CanWrite) continue;
                object value = info.GetValue(res);
                if (value == null) continue;
                info.SetValue(data, value);
            }

            //(x.3)
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in fields)
            {
                object value = info.GetValue(res);
                if (value == null) continue;
                info.SetValue(data, value);
            }

            return data;
        }
        #endregion

    }
}
