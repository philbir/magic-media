using System.Text.Json.Serialization;

namespace MagicMedia.SamsungTv;

public class SamsungTvFeatures
{
    [JsonPropertyName("filters")]
    public List<string> Filters { get; set; }

    [JsonPropertyName("mattes")]
    public List<string> Mattes { get; set; }
}
