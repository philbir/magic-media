using System;
using System.Threading.Tasks;

namespace MagicMedia
{
    public class MediaMetadata
    {
        public MediaDimension Dimension { get; set; }
        public GeoLocation? GeoLocation { get; set; }
        public string Orientation { get; set; }
        public CameraData Camera { get; set; }
        public string ImageId { get; set; }
        public DateTime? DateTaken { get; set; }
    }

    public class MediaGeoLocation
    {
        public Guid Id { get; set; }

        public GeoCoordinate Coordinates { get; set; }

        public string GeoHash { get; set; }
    }

    public class GeoBox
    {
        public GeoCoordinate NorthEast { get; set; }
        public GeoCoordinate SouthWest { get; set; }
    }


    public class GeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
