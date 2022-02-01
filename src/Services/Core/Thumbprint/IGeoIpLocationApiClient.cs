using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;

namespace MagicMedia.Thumbprint;

public class IPGeolocationApiClient : IGeoIPLocationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IPGeolocationApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GeoIpLocation> LookupAsync(string ipAddress, CancellationToken cancellationToken)
    {
        if (ipAddress.IsInternalIP())
        {
            return new GeoIpLocation
            {
                IpAddress = ipAddress
            };
        }

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
