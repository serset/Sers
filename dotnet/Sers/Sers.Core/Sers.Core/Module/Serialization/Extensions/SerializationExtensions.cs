using Sers.Core.Module.Serialization;
using Sers.Core.Util.ConfigurationManager;

namespace Sers.Core.Extensions
{
    public static partial class SerializationExtensions
    {
        /// <summary>
        /// 初始化序列化配置
        /// </summary>
        /// <param name="value"></param>
        public static void InitByConfigurationManager(this Serialization value)
        {
            #region (x.1)初始化序列化字符编码
            var encoding = ConfigurationManager.Instance.GetByPath<string>("Sers.Serialization.Encoding");
            if (!string.IsNullOrWhiteSpace(encoding))
            {
                try
                {
                    value.SetEncoding(encoding.StringToEnum<EEncoding>());
                }
                catch
                {
                }
            }
            #endregion


            #region (x.2) DateTimeFormat
            var DateTimeFormat = ConfigurationManager.Instance.GetByPath<string>("Sers.Serialization.DateTimeFormat");
            if (!string.IsNullOrWhiteSpace(DateTimeFormat))
            {
                try
                {
                    value.SetDateTimeFormat(DateTimeFormat);
                }
                catch
                {
                }
            }
            #endregion
        }

    }
}
