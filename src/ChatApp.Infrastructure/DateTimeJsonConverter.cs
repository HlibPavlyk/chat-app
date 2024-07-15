using System.Text.Json.Serialization;
using System.Text.Json;

namespace ChatApp.Infrastructure;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string DateTimeFormat = "dd MMMM yyyy, HH:mm";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeStr = reader.GetString();
        return DateTime.ParseExact(dateTimeStr, DateTimeFormat, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateTimeFormat));
    }
}
