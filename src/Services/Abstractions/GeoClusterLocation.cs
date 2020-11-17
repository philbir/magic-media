using System;

namespace MagicMedia
{
    public class GeoClusterLocation
    {
        public string Hash { get; set; }
        public GeoCoordinate Coordinates { get; set; }
        public int Count { get; set; }
        public Guid Id { get; set; }
    }
}
