using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APBD1.Utils
{
    public static class DateTimeParsing
    {
        private const string DATE_PATTERN = @"dd.MM.yyyy";
        
        public static DateTime AsDateTime(this string str)
        {
            return DateTime.ParseExact(str, DATE_PATTERN, CultureInfo.InvariantCulture);
        }

        public static string AsString(this DateTime dt)
        {
            return dt.ToString(DATE_PATTERN);
        }
        
        public class Converter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetString().AsDateTime();
            }
 
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.AsString());
            }
        }
    }
}