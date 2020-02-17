using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Vit.Extensions;
using System;
using System.Text;
using Vit.Core.Util.ConfigurationManager;

namespace Vit.Core.Module.Serialization
{
    public class Serialization
    {
        
        public static Serialization Instance { get; } = new Serialization();

        #region 静态初始化器
        static Serialization()
        {
            //初始化序列化配置
            Instance.InitByAppSettings();
        }
        #endregion


        #region 成员对象

        /// <summary>
        /// 时间序列化格式。例如 "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        readonly IsoDateTimeConverter Serialize_DateTimeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

        /// <summary>
        /// 设置时间序列化格式。例如 "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="DateTimeFormat"></param>
        public void SetDateTimeFormat(string DateTimeFormat) {
            Serialize_DateTimeFormat.DateTimeFormat = DateTimeFormat;
        }


        public Encoding encoding { get; private set; } = System.Text.Encoding.UTF8;
        public string charset => encoding.GetCharset();

        public void SetEncoding(EEncoding? type)
        {
            if (null == type) return;

            switch (type.Value)
            {
                case EEncoding.ASCII: encoding = Encoding.ASCII; return;
                case EEncoding.UTF32: encoding = Encoding.UTF32; return;
                case EEncoding.UTF7: encoding = Encoding.UTF7; return;
                case EEncoding.UTF8: encoding = Encoding.UTF8; return;
                case EEncoding.Unicode: encoding = Encoding.Unicode; return;
            }
        }
        #endregion


        #region InitByAppSettings                
        /// <summary>
        /// 根据配置文件（appsettings.json）初始化序列化配置
        /// </summary>
        /// <param name="value"></param>
        public void InitByAppSettings()
        {
            #region (x.1)初始化序列化字符编码
            var encoding = ConfigurationManager.Instance.GetByPath<string>("Vit.Serialization.Encoding");
            if (!string.IsNullOrWhiteSpace(encoding))
            {
                try
                {
                    SetEncoding(encoding.StringToEnum<EEncoding>());
                }
                catch
                {
                }
            }
            #endregion


            #region (x.2) DateTimeFormat
            var DateTimeFormat = ConfigurationManager.Instance.GetByPath<string>("Vit.Serialization.DateTimeFormat");
            if (!string.IsNullOrWhiteSpace(DateTimeFormat))
            {
                try
                {
                    SetDateTimeFormat(DateTimeFormat);
                }
                catch
                {
                }
            }
            #endregion
        }
        #endregion


        #region (x.1)bytes <--> String

        public string BytesToString(byte[] data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetString(data);
        }


        public byte[] StringToBytes(string data, Encoding encoding = null)
        {
            return (encoding ?? this.encoding).GetBytes(data);
        }
        #endregion



        #region (x.2)object <--> String
        
        #region SerializeToString

        public string SerializeToString(object value)
        {
            if (null == value) return null;

            if (value.GetType().TypeIsValueTypeOrStringType())
            {
                return value.Convert<string>();
            }

            return JsonConvert.SerializeObject(value, Serialize_DateTimeFormat);
            //return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,Serialize_DateTimeFormat);
            //return JsonConvert.SerializeObject(value, timeFormat);    
        }
        #endregion

        #region DeserializeFromString

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object DeserializeFromString(string value, Type type)
        {
            if (null == value || null == type) return null;

            if (type.TypeIsValueTypeOrStringType())
            {
                return DeserializeStruct(value, type);
            }
            return DeserializeClass(value, type);
        }


        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T DeserializeFromString<T>(string value)
        {
            return (T)DeserializeFromString(value, typeof(T));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">必须为 where T : struct</param>
        /// <returns></returns>
        private object DeserializeStruct(string value, Type type)
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
        //private object DeserializeStruct<T>(string value)
        //{
        //    return DeserializeStruct(value, typeof(T));
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"> 必须为 where T : class </param>
        /// <returns></returns>
        private object DeserializeClass(string value, Type type)
        {
            if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();
            return JsonConvert.DeserializeObject(value, type);
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T">必须为 where T : class</typeparam>
        ///// <param name="value"></param>
        //private T DeserializeClass<T>(string value)
        //{
        //    return (T)DeserializeClass(value, typeof(T));
        //}
        #endregion

        #endregion



        #region (x.3)object <--> bytes

        #region SerializeToBytes

        /// <summary>
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] SerializeToBytes(object obj)
        {
            if (null == obj) return new byte[0];
            
            if (obj is byte[] bytes)
            {
                return bytes;
            }
          
            if(! (obj is string strValue))
            {                 
                strValue = SerializeToString(obj);
            }
            return StringToBytes(strValue);
        }
        #endregion


        #region DeserializeFromBytes

        public T DeserializeFromBytes<T>(byte[] bytes)
        {
            return (T)DeserializeFromBytes(bytes, typeof(T));
        }

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
            return DeserializeFromString(strValue,type);
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
        public ArraySegment<byte> SerializeToArraySegmentByte(object obj)
        {
            if (null == obj) return ArraySegmentByteExtensions.Null;

            if (obj is ArraySegment<byte> asbs)
            {
                return asbs;
            }
            if (obj is byte[] bytes)
            {
                return bytes.SerializeToArraySegmentByte();
            }

            if (!(obj is string strValue))
            {
                strValue = SerializeToString(obj);
            }
            return StringToBytes(strValue).BytesToArraySegmentByte();
        }
        #endregion

        #region DeserializeFromArraySegmentByte

        public T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
        {
            return (T)DeserializeFromArraySegmentByte(bytes, typeof(T));
        }

        public object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type)
        {
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
        public object ConvertBySerialize(Object value, Type type)
        {
            var str = SerializeToString(value);
            return DeserializeFromString(str,type);
        }

        /// <summary>
        /// 通过序列化克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T ConvertBySerialize<T>(Object value)
        {
            var str = SerializeToString(value);
            return DeserializeFromString<T>(str);
        }
        #endregion

    }
}
