using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Vit.Core.Module.Log;
using Vit.Extensions.Json_Extensions;

namespace Vit.Core.Util.Extensible
{
    /// <summary>
    /// 可动态扩展属性，但是序列化时不处理动态扩展属性
    /// </summary>
    public class Extensible
    {
 

        [JsonIgnore]
        public IDictionary<string, object> extensionData { get; protected set; }


        /// <summary>
        /// Extensible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SetData<T>(string key,T value)
        {
            (extensionData ?? (extensionData = new Dictionary<string, object>()))[key] = value;
        }

        /// <summary>
        /// Extensible。若类型不匹配，则返回默认值（default(T)）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T GetData<T>(string key)
        {
            if (extensionData !=null && extensionData.TryGetValue(key,out var value))
            {
                if (value is T data)
                {
                    return data;
                }
                //if (typeof(T).IsAssignableFrom(value?.GetType()))
                //    return (T)value;
            }
            return default(T);
        }

        /// <summary>
        /// Extensible。若类型不匹配，则通过Convert转换。若转换不通过，则返回默认值（default(T)）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T GetDataByConvert<T>(string key)
        {
            if (extensionData != null && extensionData.TryGetValue(key, out var value))
            {
                if (value is T data)
                {
                    return data;
                }
                //if (typeof(T).IsAssignableFrom(value?.GetType()))
                //    return (T)value;

                try
                {
                    return value.Convert<T>();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }                
            }
            return default(T);
        }

        /// <summary>
        /// Extensible。若类型不匹配，则通过Serialize转换。若转换不通过，则返回默认值（default(T)）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T GetDataBySerialize<T>(string key)
        {
            if (extensionData != null && extensionData.TryGetValue(key, out var value))
            {
                if (value is T data)
                {
                    return data;
                }
                //if (typeof(T).IsAssignableFrom(value?.GetType()))
                //    return (T)value;

                try
                {
                    return value.ConvertBySerialize<T>();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return default(T);
        }

    }
}
