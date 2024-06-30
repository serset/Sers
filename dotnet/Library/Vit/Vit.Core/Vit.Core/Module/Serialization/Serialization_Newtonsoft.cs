using Newtonsoft.Json;
using System;
using Vit.Core.Util.ConfigurationManager;
using System.Runtime.CompilerServices;
using System.Text;
using Vit.Extensions.Json_Extensions;
using Newtonsoft.Json.Converters;

namespace Vit.Core.Module.Serialization
{
    public class Serialization_Newtonsoft : ISerialization
    {
        #region defaultEncoding
        public static Encoding defaultEncoding { get; set; } =
            Appsettings.json.GetByPath<string>("Vit.Serialization.Encoding")?.StringToEnum<EEncoding>().ToEncoding() ?? Encoding.UTF8;

        #endregion



        public static readonly Serialization_Newtonsoft Instance = new Serialization_Newtonsoft();





        #region Member
        public Encoding encoding { get; set; } = defaultEncoding;
        public string charset => encoding?.GetCharset();
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


        #region bytes <--> String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string BytesToString(byte[] data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetString(data);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] StringToBytes(string data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetBytes(data);
        }
        #endregion 



        #region DeserializeStruct

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">must be struct</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeStruct(string value, Type type)
        {
            try
            {
                if (type.IsStringType())
                {
                    return value;
                }
                return value.Convert(type);
            }
            catch { }
            return type.DefaultValue();
        }

        #endregion



        public readonly Newtonsoft.Json.JsonSerializerSettings serializeSetting = new Newtonsoft.Json.JsonSerializerSettings();

        public Serialization_Newtonsoft()
        {
            // ignore properties with null value when serializing
            serializeSetting.NullValueHandling = NullValueHandling.Ignore;

            // no pretty style
            serializeSetting.Formatting = Formatting.None;

            // serialize enum to string not int
            serializeSetting.Converters.Add(new StringEnumConverter());

            // DateTimeFormat
            var DateTimeFormat = Appsettings.json.GetByPath<string>("Vit.Serialization.DateTimeFormat") ?? "yyyy-MM-dd HH:mm:ss";
            serializeSetting.DateFormatHandling = global::Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            serializeSetting.DateFormatString = DateTimeFormat;
        }





        #region #1 object <--> String

        #region Serialize

        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get "null" if value is null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Serialize(object value, Type type = null)
        {
            //if (null == value) return null;

            //if (value is Newtonsoft.Json.Linq.JToken token)
            //{
            //    if (token.Type == JTokenType.String) return token.Value<string>();

            //    return token.ToString(serializeSetting.Formatting);
            //}

            ////if (value is DateTime time)
            ////{
            ////    return time.ToString(serializeSetting.DateFormatString);
            ////}

            //if (type == null) type = value.GetType();

            //if (type.IsValueType && type.GetUnderlyingTypeIfNullable() != typeof(DateTime))
            //{
            //    return value.ToString();
            //}

            return JsonConvert.SerializeObject(value, type, serializeSetting);
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// T could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get default value of T if value is null or whiteSpace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Deserialize<T>(string value)
        {
            ////return (T)Deserialize(value,typeof(T));
            //if (null == value) return default;

            //Type type = typeof(T);

            //if (type.GetUnderlyingTypeIfNullable().IsEnum)
            //    return value.StringToEnum<T>();

            ////if (type.TypeIsValueTypeOrStringType())
            //if (type.IsValueType && type.GetUnderlyingTypeIfNullable() != typeof(DateTime))
            //    return (T)DeserializeStruct(value, type);


            if (string.IsNullOrWhiteSpace(value)) return default;

            return JsonConvert.DeserializeObject<T>(value, serializeSetting);
        }


        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool) .
        /// will get default value of T if value is null or whiteSpace
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Deserialize(string value, Type type)
        {
            //if (null == value || null == type) return null;

            //if (type.GetUnderlyingTypeIfNullable().IsEnum)
            //    return value.StringToEnum(type);

            ////if (type.TypeIsValueTypeOrStringType())
            //if (type.IsValueType && type.GetUnderlyingTypeIfNullable() != typeof(DateTime))
            //    return DeserializeStruct(value, type);

            if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();

            return JsonConvert.DeserializeObject(value, type, serializeSetting);
        }

        #endregion

        #endregion





        #region #2 object <--> bytes

        #region SerializeToBytes


        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytesWithEncoding(object obj, Type type = null, Encoding encoding = null)
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
                    strValue = str;
                    break;
                default:
                    strValue = Serialize(obj, type);
                    break;
            }
            return StringToBytes(strValue, encoding);
        }



        /// <summary>
        /// value and type could be:   byte[] / string / Object / Array / struct or ValueType(int? / bool)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value, Type type = null)
        {
            return SerializeToBytesWithEncoding(value, type);
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
            return Deserialize(strValue, type);
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
            return Deserialize(strValue, type);
        }
        #endregion

        #endregion







        #region #3 object <--> ArraySegmentByte

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
        public T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
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
            return Deserialize(strValue, type);
        }
        #endregion

        #endregion




    }
}
