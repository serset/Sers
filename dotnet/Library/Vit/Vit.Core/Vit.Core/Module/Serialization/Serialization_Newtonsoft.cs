using Newtonsoft.Json;
using Vit.Extensions;
using System;
using Vit.Core.Util.ConfigurationManager;
using System.Runtime.CompilerServices;

namespace Vit.Core.Module.Serialization
{
    public class Serialization_Newtonsoft: Serialization
    {
        
        public new static readonly Serialization_Newtonsoft Instance = new Serialization_Newtonsoft().InitByAppSettings();



        public readonly Newtonsoft.Json.JsonSerializerSettings serializeSetting = new Newtonsoft.Json.JsonSerializerSettings();
        public Serialization_Newtonsoft() 
        {

            //忽略空值
            serializeSetting.NullValueHandling = NullValueHandling.Ignore;

            //不缩进
            serializeSetting.Formatting = Formatting.None;

            //日期格式化
            serializeSetting.DateFormatHandling = global::Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            serializeSetting.DateFormatString = "yyyy-MM-dd HH:mm:ss";



            //Newtonsoft.Json.JsonSerializerSettings serializeSetting = new Newtonsoft.Json.JsonSerializerSettings();
            //JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            //{
            //    //日期类型默认格式化处理
            //    //serializeSetting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
            //    //serializeSetting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //    //空值处理
            //    serializeSetting.NullValueHandling = NullValueHandling.Ignore;
            //    //serializeSetting.Converters.Add(Serialize_DateTimeFormat);
            //    return serializeSetting;
            //});
        }


        #region 成员对象

        /// <summary>
        /// 时间序列化格式。例如 "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        //readonly IsoDateTimeConverter Serialize_DateTimeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

        /// <summary>
        /// 设置时间序列化格式。例如 "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="DateTimeFormat"></param>
        public  void SetDateTimeFormat(string DateTimeFormat) 
        {
            //Serialize_DateTimeFormat.DateTimeFormat = DateTimeFormat;

            serializeSetting.DateFormatString = DateTimeFormat;
        }

 

       
        #endregion


        #region InitByAppSettings
        /// <summary>
        /// 根据配置文件（appsettings.json）初始化序列化配置
        /// </summary>
        public Serialization_Newtonsoft InitByAppSettings()
        { 

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

            return this;
        }
        #endregion


      


        #region (x.2)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string SerializeToString<T>(T value)
        {
            if (null == value) return null;

            if (value.GetType().TypeIsValueTypeOrStringType())
            {
                return value.Convert<string>();
            }
            //return JsonConvert.SerializeObject(value, Serialize_DateTimeFormat);     
            return JsonConvert.SerializeObject(value, serializeSetting); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string SerializeToString(object value, Type type)
        {
            //return JsonConvert.SerializeObject(value, Serialize_DateTimeFormat);
            return JsonConvert.SerializeObject(value, serializeSetting);
        }

        #endregion

        #region DeserializeFromString

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object DeserializeFromString(string value, Type type)
        {
            if (null == value || null == type) return null;

            if (type.TypeIsValueTypeOrStringType())
            {
                return DeserializeStruct(value, type);
            }

            if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();

            return JsonConvert.DeserializeObject(value, type);
        }

        #endregion

        #endregion







    }
}
