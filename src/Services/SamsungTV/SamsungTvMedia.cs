using System.Text.Json.Serialization;

namespace MagicMedia.SamsungTv;

public class SamsungTvMedia
{
    [JsonPropertyName("content_id")] public string Id { get; set; }

    [JsonPropertyName("category_id")] public string CategoryId { get; set; }

    [JsonPropertyName("content_type")] public string ContentType { get; set; }

    [JsonPropertyName("height")] public int Height { get; set; }

    [JsonPropertyName("width")] public int Width { get; set; }

    [JsonPropertyName("image_date")] public DateTime? ImageDate { get; set; }

    [JsonPropertyName("matte_id")] public string MatteId { get; set; }

    [JsonPropertyName("portrait_matte_id")]
    public string PortraitMatteId { get; set; }

    [JsonPropertyName("selected")] public bool Selected { get; set; }
}
