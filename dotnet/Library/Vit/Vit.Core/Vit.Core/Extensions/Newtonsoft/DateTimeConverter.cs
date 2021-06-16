using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public class DateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

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
