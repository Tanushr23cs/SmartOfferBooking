using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartOfferBooking.API.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            throw new JsonException("DateOnly value cannot be null or empty");
        }

        if (DateOnly.TryParseExact(stringValue, Format, null, System.Globalization.DateTimeStyles.None, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert \"{stringValue}\" to DateOnly with format \"{Format}\".");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
