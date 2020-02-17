using System.Reflection;

namespace Vit.Extensions.ObjectExt
{
    public static partial class ObjectExtExtensions
    {

        #region CopyProrertyFrom
        /// <summary>
        /// 通过反射复制对象的同名属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <returns></returns>
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
            foreach (var propertyRes in typeRes.GetProperties())
            {
                var propertyDest = typeDest.GetProperty(propertyRes.Name);
                if (propertyDest != null)
                {
                    var value = propertyRes.GetValue(res);
                    if (value != null)
                    {
                        //value = value.Convert(propertyDest.PropertyType);
                        propertyDest.SetValue(data, value);
                    }
                }
            }

            //(x.3)
            foreach (var propertyRes in typeRes.GetFields())
            {
                var propertyDest = typeDest.GetField(propertyRes.Name);
                if (propertyDest != null)
                {
                    var value = propertyRes.GetValue(res);
                    if (value != null)
                    {
                        //value = value.Convert(propertyDest.FieldType);
                        propertyDest.SetValue(data, value);
                    }
                }
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
            foreach (var propertyRes in typeRes.GetProperties())
            {
                var propertyDest = typeDest.GetProperty(propertyRes.Name);
                if (propertyDest != null)
                {
                    var value = propertyRes.GetValue(res);
                    if (value != null)
                    {
                        //value = value.Convert(propertyDest.PropertyType);
                        propertyDest.SetValue(data, value);
                    }
                }
            }

            //(x.3)
            foreach (var propertyRes in typeRes.GetFields())
            {
                var propertyDest = typeDest.GetField(propertyRes.Name);
                if (propertyDest != null)
                {
                    var value = propertyRes.GetValue(res);
                    if (value != null)
                    {
                        //value = value.Convert(propertyDest.FieldType);
                        propertyDest.SetValue(data, value);
                    }
                }
            }
            return data;
        }
        #endregion



        #region  Field

        /// <summary>
        /// 获得字段的值(包含私有)
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="instance">扩展对象类型</param>
        /// <param name="fieldName">私有字段名称 string</param>
        /// <returns>私有字段值 T</returns>
        public static T GetField<T>(this object instance, string fieldName)
        {          
            return (T)instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
        }

        /// <summary>
        /// 设置字段的值(包含私有)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldname">字段名称 string</param>
        /// <param name="value">私有字段新值 object</param>
        public static void SetField(this object instance, string fieldname, object value)
        {             
            instance.GetType().GetField(fieldname, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);
        }

        #endregion

        #region Property

        /// <summary>
        /// 获取属性的值(包含私有)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName">属性名称 string</param>
        /// <returns>私有字段值 T</returns>
        public static object GetProperty(this object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
        }



        /// <summary>
        /// 获取属性的值(包含私有)
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName">属性名称 string</param>
        /// <returns>私有字段值 T</returns>
        public static T GetProperty<T>(this object instance, string propertyName)
        {
            return (T)GetProperty(instance, propertyName); 
        }

        /// <summary>
        /// 设置属性的值(包含私有)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value"></param>
        public static void SetProperty(this object instance, string propertyName, object value)
        {
            instance.GetType().GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);            
        }

        #endregion


        #region Method

        /// <summary>
        /// 调用方法(包含私有)
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="instance"></param>
        /// <param name="methodName">方法名称</param>
        /// <param name="param">参数列表</param>
        /// <returns>调用方法返回值</returns>
        public static T CallMethod<T>(this object instance, string methodName, params object[] param)
        {      
            return (T)instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(instance, param);
        }

        #endregion

    }
}
