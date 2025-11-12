using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared
{
    public class TimeOnlyNewtonsoftConverter : Newtonsoft.Json.JsonConverter
    {
        private static readonly string[] AcceptedFormats = new[]
        {
            "HH:mm:ss.FFFFFFF",
            "HH:mm:ss",
            "HH:mm",
            "H:mm"
        };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeOnly) || objectType == typeof(TimeOnly?);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            // Handle null
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType == typeof(TimeOnly?))
                    return null;
                throw new JsonSerializationException("Cannot convert null value to TimeOnly.");
            }

            // String values like "19:30" or "19:30:00"
            if (reader.TokenType == JsonToken.String)
            {
                var s = (reader.Value as string)?.Trim();
                if (string.IsNullOrEmpty(s))
                {
                    if (objectType == typeof(TimeOnly?)) return null;
                    throw new JsonSerializationException("Cannot convert empty string to TimeOnly.");
                }

                if (TimeOnly.TryParseExact(s, AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                    return time;

                if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                    return time;

                throw new JsonSerializationException($"Unable to parse TimeOnly from string '{s}'.");
            }

            // Date token (DateTime) -> extract time part
            if (reader.TokenType == JsonToken.Date)
            {
                var dt = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
                return TimeOnly.FromDateTime(dt);
            }

            // Numeric tokens: interpret as ticks if large, otherwise seconds since midnight
            if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
            {
                var raw = Convert.ToInt64(reader.Value);
                // Heuristic: if value looks like ticks use ticks, else treat as seconds
                if (raw > TimeSpan.TicksPerDay)
                {
                    return TimeOnly.FromTimeSpan(TimeSpan.FromTicks(raw));
                }
                else
                {
                    return TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(raw));
                }
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing TimeOnly.");
        }

        public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var time = (TimeOnly)value;
            // Write standardized time string with seconds
            writer.WriteValue(time.ToString("HH:mm", CultureInfo.InvariantCulture));
        }
    }
}
