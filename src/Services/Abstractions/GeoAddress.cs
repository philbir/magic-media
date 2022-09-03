namespace MagicMedia;

public class GeoAddress
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public string? Distric1 { get; set; }
    public string? Distric2 { get; set; }
    public string? EntityType { get; set; }
    public RawGeoAddress Raw { get; set; }
}

public class RawGeoAddress
{
    public string Data { get; set; }

    public string Format { get; set; }
}
