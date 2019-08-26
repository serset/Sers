using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sers.Core.Extensions;
using System;
using System.Text;

namespace Sers.Core.Module.Serialization
{
    public class Serialization
    {

        public static Serialization Instance { get; } = new Serialization();


        #region member
       
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



        #region SerializeToBytes

        /// <summary>
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] Serialize(object obj)
        {
            if (null == obj) return new byte[0];

            var type = obj.GetType();
            if (type == typeof(byte[]))
            {
                return (byte[])obj;
            }

            string strValue;
            if (type == typeof(string))
            {
                strValue = (string)obj;
            }
            else
            {
                strValue = obj.Serialize();
            }
            return strValue.StringToBytes();
        }
        #endregion

        #region DeserializeFromBytes

        public T Deserialize<T>(ArraySegment<byte> bytes)
        {
            return (T)Deserialize(bytes, typeof(T));
        }

        public object Deserialize(ArraySegment<byte> bytes, Type type)
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
            return strValue.Deserialize(type);
        }
        #endregion

        

        #region ConvertBySerialize

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
