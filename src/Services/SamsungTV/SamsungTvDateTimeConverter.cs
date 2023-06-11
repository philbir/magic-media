using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MagicMedia.SamsungTv;

public class SamsungTvDateTimeConverter : JsonConverter<DateTime?>
{
    private const string _format = "yyyy:MM:dd HH:mm:ss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return DateTime.ParseExact(reader.GetString(), _format, CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString(_format));
    }
}
