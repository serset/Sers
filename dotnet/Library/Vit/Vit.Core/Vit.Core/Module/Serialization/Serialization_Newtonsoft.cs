using Newtonsoft.Json;
using Vit.Extensions;
using System;
using Vit.Core.Util.ConfigurationManager;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Vit.Core.Module.Serialization
{
    public class Serialization_Newtonsoft: ISerialization
    {
        #region defaultEncoding
        public static Encoding defaultEncoding { get; set; } =
            Appsettings.json.GetByPath<string>("Vit.Serialization.Encoding")?.StringToEnum<EEncoding>().ToEncoding() ?? Encoding.UTF8;

        #endregion



        public static readonly Serialization_Newtonsoft Instance = new Serialization_Newtonsoft();
        


        #region 基础对象

      

        #region 成员对象 


        public Encoding encoding { get; set; } = defaultEncoding;
        public string charset { get => encoding.GetCharset(); }

        #endregion



        #region SpanToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SpanToString(ReadOnlySpan<byte> data, Encoding encoding = null)
        {
            if (data.Length == 0) return default;
            unsafe
            {
                fixed (byte* bytes = data)
                {
                    return (encoding ?? this.encoding).GetString(bytes, data.Length);
                }
            }
        }

        #endregion


        #region (x.1)bytes <--> String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string BytesToString(byte[] data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetString(data);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  byte[] StringToBytes(string data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetBytes(data);
        }
        #endregion



        #region DeserializeStruct

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">必须为 where T : struct</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeStruct(string value, Type type)
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T">必须为 where T : struct</typeparam>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public object DeserializeStruct<T>(string value)
        //{
        //    return DeserializeStruct(value, typeof(T));
        //}



        #endregion

        #endregion







        public readonly Newtonsoft.Json.JsonSerializerSettings serializeSetting = new Newtonsoft.Json.JsonSerializerSettings();

        public Serialization_Newtonsoft() 
        {

            //忽略空值
            serializeSetting.NullValueHandling = NullValueHandling.Ignore;

            //不缩进
            serializeSetting.Formatting = Formatting.None;

            //日期格式化
            var DateTimeFormat = Appsettings.json.GetByPath<string>("Vit.Serialization.DateTimeFormat")
                ?? "yyyy-MM-dd HH:mm:ss";

            serializeSetting.DateFormatHandling = global::Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            serializeSetting.DateFormatString = DateTimeFormat;
        }





        #region (x.1)object <--> String

        #region SerializeToString

        /// <summary>
        /// T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString<T>(T value)
        {
            if (null == value) return null;
         
            return SerializeToString(value, value.GetType()); 
        }

        /// <summary>
        /// T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(object value, Type type)
        {
            if (null == value) return null;

            if (value is Newtonsoft.Json.Linq.JToken token)
            {             
                if(token.TypeMatch(JTokenType.String)) return token.ToString();

                return token.ToString(serializeSetting.Formatting);
            }

            if (type.TypeIsValueTypeOrStringType())
            {
                //return value.Convert<string>();
                return value.ToString();
            }
            return JsonConvert.SerializeObject(value, serializeSetting);
        }

        #endregion

        #region DeserializeFromString

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>    
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromString<T>(string value)
        {
            //return (T)DeserializeFromString(value,typeof(T));
            if (null == value) return default;

            Type type = typeof(T);

            if (type.GetUnderlyingTypeIfNullable().IsEnum)
                return value.StringToEnum<T>();

            if (type.TypeIsValueTypeOrStringType())
                return (T)DeserializeStruct(value, type);


            //if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();

            return JsonConvert.DeserializeObject<T>(value);
        }


        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromString(string value, Type type)
        {
            if (null == value || null == type) return null;

            if (type.GetUnderlyingTypeIfNullable().IsEnum)
                return value.StringToEnum(type);

            if (type.TypeIsValueTypeOrStringType())
                return DeserializeStruct(value, type);

            //if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();

            return JsonConvert.DeserializeObject(value, type);
        }

        #endregion

        #endregion







        #region (x.2)object <--> bytes

        #region SerializeToBytes
        /// <summary>
        /// T 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes<T>(T obj) 
        {
            return SerializeToBytes<T>(obj,null);
        }

        /// <summary>
        /// T 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes<T>(T obj, Encoding encoding)
        {
            if (null == obj) return null;

            string strValue;
            switch (obj)
            {
                case byte[] bytes:
                    return bytes;
                case ArraySegment<byte> asbs:
                    return asbs.ArraySegmentByteToBytes();
                case string str:
                    strValue = str; break;
                default: strValue = SerializeToString(obj); break;
            }

            return StringToBytes(strValue, encoding);
        }



        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value, Type type)
        {
            return SerializeToBytes(value);
        }
        #endregion


        #region DeserializeFromBytes

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromBytes<T>(byte[] bytes)
        {
            return (T)DeserializeFromBytes(bytes, typeof(T));
        }

        /// <summary>
        /// type 可以为   byte[]、string、 object 、struct
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromBytes(byte[] bytes, Type type)
        {
            if (type == typeof(byte[]))
            {
                return bytes;
            }
            if (type == typeof(ArraySegment<byte>))
            {
                return bytes.BytesToArraySegmentByte();
            }
            string strValue = BytesToString(bytes);
            if (type == typeof(string))
            {
                return strValue;
            }
            return DeserializeFromString(strValue, type);
        }
        #endregion


        #region DeserializeFromSpan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromSpan<T>(ReadOnlySpan<byte> bytes)
        {
            return (T)DeserializeFromSpan(bytes, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromSpan(ReadOnlySpan<byte> bytes, Type type)
        {
            if (bytes.Length == 0) return default;

            string strValue = SpanToString(bytes);
            if (type == typeof(string))
            {
                return strValue;
            }
            return DeserializeFromString(strValue, type);
        }
        #endregion

        #endregion










        #region (x.4)object <--> ArraySegmentByte

        #region SerializeToArraySegmentByte
        /// <summary>
        /// obj 可以为   byte[]、string、 object 、ArraySegment    
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> SerializeToArraySegmentByte(object obj)
        {
            if (null == obj) return ArraySegmentByteExtensions.Null;

            if (obj is ArraySegment<byte> asbs)
            {
                return asbs;
            }
            if (obj is byte[] bytes)
            {
                return bytes.BytesToArraySegmentByte();
            }

            return SerializeToBytes(obj).BytesToArraySegmentByte();
        }
        #endregion




        #region DeserializeFromArraySegmentByte
 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
        {
            if (bytes.Count == 0) return default;
            return (T)DeserializeFromArraySegmentByte(bytes, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type)
        {
            if (bytes.Count == 0) return default;

            if (type == typeof(byte[]))
            {
                return bytes.ArraySegmentByteToBytes();
            }
            if (type == typeof(ArraySegment<byte>))
            {
                return bytes;
            }
            string strValue = bytes.ArraySegmentByteToString();
            if (type == typeof(string))
            {
                return strValue;
            }
            return DeserializeFromString(strValue, type);
        }
        #endregion

        #endregion




    }
}
