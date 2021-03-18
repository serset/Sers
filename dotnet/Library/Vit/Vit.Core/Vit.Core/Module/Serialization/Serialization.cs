using Vit.Extensions;
using System;
using System.Text;
using System.Runtime.CompilerServices;
using Vit.Core.Util.ConfigurationManager;


namespace Vit.Core.Module.Serialization
{
    public  abstract class Serialization
    {

        #region defaultEncoding
        public static Encoding defaultEncoding { get; set; } =
            ConfigurationManager.Instance.GetByPath<string>("Vit.Serialization.Encoding")?.StringToEnum<EEncoding>().ToEncoding() ?? Encoding.UTF8;

        #endregion



        public static Serialization Instance { get; set; } = Serialization_Newtonsoft.Instance;

        public static readonly Serialization_Newtonsoft Newtonsoft = Serialization_Newtonsoft.Instance;
        public static readonly Serialization_Text Text = Serialization_Text.Instance;


 



        #region 成员对象 


        public virtual Encoding encoding { get; set; } = defaultEncoding;
        public virtual string charset { get => encoding.GetCharset();  }
 
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
        public virtual string BytesToString(byte[] data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetString(data);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual byte[] StringToBytes(string data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetBytes(data);
        }
        #endregion

       


        #region (x.2)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract string SerializeToString<T>(T value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract string SerializeToString(object value, Type type);        

        #endregion




        #region DeserializeFromString

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract object DeserializeFromString(string value, Type type);
         


        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T DeserializeFromString<T>(string value)
        {
            return (T)DeserializeFromString(value, typeof(T));
        }


           
        #endregion

        #endregion



        #region (x.3)object <--> bytes

        #region SerializeToBytes

        /// <summary>
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual byte[] SerializeToBytes<T>(T obj)
        {
            string strValue;
            switch (obj) 
            {
                case null:
                    return new byte[0];
                case byte[] bytes:
                    return bytes;
                case ArraySegment<byte> asbs:
                    return asbs.ArraySegmentByteToBytes();
                case string str:
                    strValue = str; break;
                default: strValue = SerializeToString(obj);break;
            }    
      
            return StringToBytes(strValue);
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual byte[] SerializeToBytes(object value, Type type) 
        {
            return SerializeToBytes(value);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T DeserializeFromBytes<T>(byte[] bytes)
        {
            return (T)DeserializeFromBytes(bytes, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual object DeserializeFromBytes(byte[] bytes, Type type)
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
            return DeserializeFromString(strValue,type);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T DeserializeFromSpan<T>(ReadOnlySpan<byte> bytes)
        {
            return (T)DeserializeFromSpan(bytes, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual object DeserializeFromSpan(ReadOnlySpan<byte> bytes, Type type)
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
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual ArraySegment<byte> SerializeToArraySegmentByte<T>(T obj)
        {
            if (null == obj) return ArraySegmentByteExtensions.Null;

            if (obj is ArraySegment<byte> asbs)
            {
                return asbs;
            }
            
            return SerializeToBytes(obj).BytesToArraySegmentByte();
        }
        #endregion

        #region DeserializeFromArraySegmentByte

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
        {
            if (bytes.Count == 0) return default;
            return (T)DeserializeFromArraySegmentByte(bytes, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type)
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
            return DeserializeFromString(strValue,type);
        }
        #endregion

        #endregion


        #region (x.5)ConvertBySerialize

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual object ConvertBySerialize(Object value, Type type)
        {
            var str = SerializeToString(value);
            return DeserializeFromString(str,type);
        }

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary> 
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual TTarget ConvertBySerialize<TTarget>(Object value)
        {
            var str = SerializeToString(value);
            return DeserializeFromString<TTarget>(str);
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
        public  object DeserializeStruct(string value, Type type)
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

    }
}
