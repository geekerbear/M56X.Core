using System.Text.Json.Serialization;
using System.Text.Json;

namespace M56X.Core.Json
{
    /// <summary>
    /// 日期格式转换器
    /// </summary>
    /// <param name="format"></param>
    public class JsonDateTimeConverter(string format = "yyyy-MM-dd HH:mm:ss") : JsonConverter<DateTime>
    {
        private readonly string _format = format;

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString() ?? "1970-01-01 00:00:00", _format, System.Globalization.CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
