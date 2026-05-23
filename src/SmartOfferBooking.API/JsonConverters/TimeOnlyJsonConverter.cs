using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartOfferBooking.API.JsonConverters;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private const string Format = "HH:mm:ss";

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            throw new JsonException("TimeOnly value cannot be null or empty");
        }

        if (TimeOnly.TryParseExact(stringValue, Format, null, System.Globalization.DateTimeStyles.None, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert \"{stringValue}\" to TimeOnly with format \"{Format}\".");
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
