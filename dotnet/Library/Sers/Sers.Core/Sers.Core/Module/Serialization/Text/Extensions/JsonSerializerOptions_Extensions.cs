using System.Text.Json;

using Sers.Core.Module.Serialization.Text;

namespace Vit.Extensions
{
    public static partial class JsonSerializerOptions_Extensions
    {


        #region AddConverter_Newtonsoft

        public static JsonSerializerOptions AddConverter_Newtonsoft(this JsonSerializerOptions options)
        {
            if (options == null) return options;

            options.Converters.Add(JsonConverter_JObject.Instance);
            options.Converters.Add(JsonConverter_JArray.Instance);

            return options;
        }

        #endregion



        #region AddConverter_DateTime

        public static JsonConverter_DateTime AddConverter_DateTime(this JsonSerializerOptions options, string dateFormatString = "yyyy-MM-dd HH:mm:ss")
        {
            if (options == null) return null;

            var jsonConverter_DateTime = new JsonConverter_DateTime();
            jsonConverter_DateTime.dateFormatString = dateFormatString;
            options.Converters.Add(jsonConverter_DateTime);

            return jsonConverter_DateTime;
        }

        #endregion

    }
}
