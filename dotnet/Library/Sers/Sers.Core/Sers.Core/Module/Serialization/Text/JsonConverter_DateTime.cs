using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sers.Core.Module.Serialization.Text
{

    public class JsonConverter_DateTime : JsonConverter<DateTime>
    {
        public string dateFormatString;

        public JsonConverter_DateTime(string dateFormatString = "yyyy-MM-dd HH:mm:ss")
        {
            this.dateFormatString = dateFormatString;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(dateFormatString));
        }
    }

}

