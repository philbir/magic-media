using System;
using System.Threading.Tasks;

namespace MagicMedia
{
    public class MediaMetadata
    {
        public MediaDimension Dimension { get; set; }
        public GeoLocation GeoLocation { get; set; }
        public string Orientation { get; set; }
        public CameraData Camera { get; set; }
        public string ImageId { get; set; }
        public DateTime? DateTaken { get; set; }
    }

    public class MediaDimension
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class CameraData
    {
        public string Model { get; set; }

        public string Make { get; set; }
    }

    public class GeoLocation
    {
        public GeoPoint Point { get; set; }

        public int Altitude { get; set; }

        public string Type { get; set; }

        public string GeoHash { get; set; }
        public GeoAddress Address { get; set; }
    }

    public class GeoPoint
    {
        public string Type { get; set; } = "Point";

        public double[] Coordinates { get; set; }

        public static GeoPoint Create(double lat, double lon)
        {
            return new GeoPoint()
            {
                Coordinates = new[] { lon, lat }
            };
        }
    }

    public class GeoAddress
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Distric1 { get; set; }
        public string Distric2 { get; set; }
        public string EntityType { get; set; }
    }

    public enum MediaOrientation
    {
        Landscape,
        Portrait
    }
}
