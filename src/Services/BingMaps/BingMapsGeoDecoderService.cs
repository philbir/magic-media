using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingMapsRESTToolkit;

namespace MagicMedia.BingMaps;

public class BingMapsGeoDecoderService : IGeoDecoderService
{
    private readonly BingMapsOptions _options;

    public BingMapsGeoDecoderService(BingMapsOptions options)
    {
        _options = options;
    }

    public async Task<GeoAddress?> DecodeAsync(
        double latitude,
        double longitude, CancellationToken cancellationToken)
    {
        ReverseGeocodeRequest request = new ReverseGeocodeRequest();
        request.BingMapsKey = _options.ApiKey;
        request.Point = new Coordinate()
        {
            Latitude = latitude,
            Longitude = longitude
        };
        request.IncludeIso2 = true;
        request.IncludeNeighborhood = true;
        request.IncludeEntityTypes = new List<EntityType>()
            {   EntityType.Address,
                EntityType.CountryRegion,
                EntityType.Postcode1,
                EntityType.PopulatedPlace,
                EntityType.Neighborhood,
                EntityType.AdminDivision1,
                EntityType.AdminDivision2 };

        Response res = await ServiceManager.GetResponseAsync(request);

        if (res.StatusCode == 200)
        {
            List<Location> locations = GetLocationsFromBingResponse(res);
            return GetAddressFromLocations(locations);
        }

        return null;
    }

    private GeoAddress? GetAddressFromLocations(List<Location> locations)
    {
        Location firstLoc = locations.FirstOrDefault();
        if (firstLoc != null)
        {
            var geoAddress = new GeoAddress();
            geoAddress.Name = firstLoc.Name;
            geoAddress.Address = firstLoc.Address?.AddressLine;
            geoAddress.City = firstLoc.Address?.Locality;
            geoAddress.CountryCode = firstLoc.Address?.CountryRegionIso2;
            geoAddress.Country = firstLoc.Address?.CountryRegion;
            geoAddress.Distric1 = firstLoc.Address?.AdminDistrict;
            geoAddress.Distric2 = firstLoc.Address?.AdminDistrict2;
            geoAddress.EntityType = firstLoc.EntityType;
            return geoAddress;
        }

        return null;
    }

    private List<Location> GetLocationsFromBingResponse(Response response)
    {
        var locations = new List<Location>();

        ResourceSet firstSet = response.ResourceSets?.FirstOrDefault();
        if (firstSet != null)
        {
            foreach (Resource res in firstSet.Resources)
            {
                var loc = res as Location;
                if (loc != null)
                {
                    locations.Add(loc);
                }
            }
        }

        return locations;
    }
}
