using System;
using System.Runtime.CompilerServices;

namespace Vit.Core.Module.Serialization
{
    public static partial class Json
    {

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
            return Instance.Serialize<T>(value);
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
            return Instance.Serialize(value, type);
        }



        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>    
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeFromString<T>(string value)
        {
            return Instance.Deserialize<T>(value);
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
            return Instance.Deserialize(value, type);
        }

        #endregion

    }
}
