namespace MagicMedia.Thumbprint
{
    public class GeoIpLocation
    {
        public string? IpAddress { get; set; }

        public GeoPoint? Point { get; set; }

        public string? ZipCode { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? CountryCode { get; set; }

        public string? Continent { get; set; }

        public string? Isp { get; set; }

        public string? Organization { get; set; }
    }
}
