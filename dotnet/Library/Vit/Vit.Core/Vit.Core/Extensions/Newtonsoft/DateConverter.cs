using System;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vit.Extensions.Newtonsoft_Extensions
{
    public class DateConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }
}
