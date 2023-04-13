using System.Reflection;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Object_Extensions
{
    public static partial class Object_Copy_Extensions
    {


        #region CopyProrertyFrom
        /// <summary>
        /// 通过反射复制对象的同名属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CopyProrertyFrom<T>(this T data, object res)
        {

            if (data == null || res == null)
            {
                return data;
            }

            //(x.1)
            var typeDest = data.GetType();
            var typeRes = res.GetType();


            //(x.2)
            foreach (PropertyInfo from in typeRes.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!from.CanRead) continue;
                PropertyInfo to = typeDest.GetProperty(from.Name, BindingFlags.Public | BindingFlags.Instance);
                if (to == null || !to.CanWrite) continue;

                object value = from.GetValue(res);
                //if (value == null) continue;

                //value = value.Convert(propertyDest.PropertyType);
                to.SetValue(data, value);
            }


            //(x.3)
            foreach (FieldInfo from in typeRes.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                FieldInfo to = typeDest.GetField(from.Name, BindingFlags.Public | BindingFlags.Instance);
                if (to == null) continue;

                object value = from.GetValue(res);
                //if (value == null) continue;

                //value = value.Convert(propertyDest.FieldType);
                to.SetValue(data, value);
            }

            return data;
        }
        #endregion


        #region CopyNotNullProrertyFrom
        /// <summary>
        /// 通过反射复制对象的非空同名属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CopyNotNullProrertyFrom<T>(this T data, object res)
        {
            if (data == null || res == null)
            {
                return data;
            }

            //(x.1)
            var typeDest = data.GetType();
            var typeRes = res.GetType();


            //(x.2)
            foreach (PropertyInfo from in typeRes.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!from.CanRead) continue;
                PropertyInfo to = typeDest.GetProperty(from.Name, BindingFlags.Public | BindingFlags.Instance);
                if (to == null || !to.CanWrite) continue;

                object value = from.GetValue(res);
                if (value == null) continue;

                //value = value.Convert(propertyDest.PropertyType);
                to.SetValue(data, value);
            }


            //(x.3)
            foreach (FieldInfo from in typeRes.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                FieldInfo to = typeDest.GetField(from.Name, BindingFlags.Public | BindingFlags.Instance);
                if (to == null) continue;

                object value = from.GetValue(res);
                if (value == null) continue;

                //value = value.Convert(propertyDest.FieldType);
                to.SetValue(data, value);
            }

            return data;
        }
        #endregion

    }
}
