namespace MagicMedia;

public class GeoPoint
{
    public string Type { get; set; } = "Point";

    public double[]? Coordinates { get; set; }

    public static GeoPoint Create(double lat, double lon)
    {
        return new GeoPoint()
        {
            Coordinates = new[] { lon, lat }
        };
    }
}
