using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint
{
    public class IPGeolocationApiClient : IPGeoLocationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IPGeolocationApiClient(IPGeolocationApiOptions options, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GeoIpLocation> LookupAsync(string ipAddress, CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("GeoIP");

            GeoIpLocationResponse? response = await client.GetFromJsonAsync<GeoIpLocationResponse>(
                $"ipgeo?ip={ipAddress}",
                cancellationToken);

            return MapToGeoIpLocation(response);
        }

        private GeoIpLocation MapToGeoIpLocation(GeoIpLocationResponse response)
        {
            var location = new GeoIpLocation
            {
                IpAddress = response.Ip,
                City = response.City,
                Continent = response.ContinentName,
                Country = response.CountryName,
                CountryCode = response.CountryCode2,
                ZipCode = response.Zipcode,
                Isp = response.Isp,
                Organization = response.Organization,
            };

            if (response.Latitude.HasValue && response.Longitude.HasValue)
            {
                location.Point = GeoPoint.Create(response.Latitude.Value, response.Longitude.Value);
            }

            return location;
        }
    }
}
