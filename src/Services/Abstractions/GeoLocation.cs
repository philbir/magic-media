namespace MagicMedia
{
    public class GeoLocation
    {
        public GeoPoint Point { get; set; }

        public int Altitude { get; set; }

        public string Type { get; set; }

        public string GeoHash { get; set; }

        public GeoAddress? Address { get; set; }
    }
}
