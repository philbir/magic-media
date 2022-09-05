using System.Net.Http.Json;
using System.Text.Json;

namespace MagicMedia.GoogleMaps;

public class GoogleMapsGeoDecoderService : IGeoDecoderService
{
    private readonly GoogleMapsOptions _options;

    public GoogleMapsGeoDecoderService(GoogleMapsOptions options)
    {
        _options = options;
    }

    public async Task<GeoAddress> DecodeAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();

        var uri = "https://maps.googleapis.com/maps/api/geocode/" +
            $"json?latlng={latitude},{longitude}&key={_options.ApiKey}";

        GeoEncodingResponse? response = await httpClient.GetFromJsonAsync<GeoEncodingResponse>(
            uri,
            cancellationToken);

        var geoAddress = new GeoAddress
        {
            Raw = new RawGeoAddress
            {
                Format = "GoogleMaps",
                Data = JsonSerializer.Serialize(response)
            }
        };

        if (response.Status != "OK")
        {
            return null;
        }

        Result? location = GetLocation(response);

        if (location is { })
        {
            geoAddress.EntityType = location.Geometry.LocationType;

            AddressComponent? country = location.AddressComponents.GetAddressComponent("country");

            if (country is { })
            {
                geoAddress.Country = country.LongName;
                geoAddress.CountryCode = country.ShortName;
            }

            geoAddress.Distric1 = location.AddressComponents.GetAddressComponentName("administrative_area_level_1");
            geoAddress.Distric2 = location.AddressComponents.GetAddressComponentName("administrative_area_level_2");
            geoAddress.City = location.AddressComponents.GetAddressComponentName("locality");

            var street = location.AddressComponents.GetAddressComponentName("route");
            if (street is { })
            {
                var nr = location.AddressComponents.GetAddressComponentName("street_number");

                if (nr is { })
                {
                    street = street + " " + nr;
                }

                geoAddress.Address = street;
            }

            geoAddress.Name = location.FormattedAddress;
        }

        return geoAddress;
    }


    private Result? GetLocation(GeoEncodingResponse data)
    {
        string[] locationTypes = { "ROOFTOP", "GEOMETRIC_CENTER" };

        foreach ( var type in locationTypes)
        {
            Result? location = data.Results.FirstOrDefault(x => x.Geometry.LocationType == type);

            if ( location is { })
            {
                return location;
            }
        }

        return null;
    }
}
