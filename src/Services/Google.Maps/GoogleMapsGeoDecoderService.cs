using System.Net.Http.Json;

namespace MagicMedia
{
    public class GoogleMapsGeoDecoderService : IGeoDecoderService
    {
        public async Task<GeoAddress> DecodeAsync(
            double latitude,
            double longitude,
            CancellationToken cancellationToken)
        {

            var httpClient = new HttpClient();

            var uri = "https://maps.googleapis.com/maps/api/geocode/" +
                "json?latlng=13.520556,99.958333&key=AIzaSyABWHk7OCoqhgv1m_2vsZiufkid1Vg30r8";

            var response = await httpClient.GetFromJsonAsync<GeoEncodingResponse>(uri, cancellationToken);

            return new GeoAddress();
        }
    }
}
