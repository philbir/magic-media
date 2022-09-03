using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb;

public class GeoDecoderCacheStore : IGeoDecoderService
{
    private readonly MediaStoreContext _mediaStoreContext;
    private readonly IGeoDecoderService _geoDecoderService;

    public GeoDecoderCacheStore(
        MediaStoreContext mediaStoreContext,
        IGeoDecoderService geoDecoderService)
    {
        _mediaStoreContext = mediaStoreContext;
        _geoDecoderService = geoDecoderService;
    }
    public async Task<GeoAddress> DecodeAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        string id = $"{latitude}_{longitude}";

        GeoAddressCache cached = await _mediaStoreContext.GeoAddressCache.AsQueryable()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (cached != null)
        {
            return new GeoAddress
            {
                Address = cached.Address,
                City = cached.City,
                Country = cached.Country,
                CountryCode = cached.CountryCode,
                Distric1 = cached.Distric1,
                Distric2 = cached.Distric2,
                EntityType = cached.EntityType,
                Name = cached.Name,
            };
        }

        GeoAddress geoAddress = await _geoDecoderService.DecodeAsync(
            latitude,
            longitude,
            cancellationToken);

        if (geoAddress != null)
        {
            cached = new GeoAddressCache
            {
                Address = geoAddress.Address,
                City = geoAddress.City,
                Country = geoAddress.Country,
                CountryCode = geoAddress.CountryCode,
                Distric1 = geoAddress.Distric1,
                Distric2 = geoAddress.Distric2,
                EntityType = geoAddress.EntityType,
                Name = geoAddress.Name,
                Raw = geoAddress.Raw,
                Id = id
            };
        }
        else
        {
            cached = new GeoAddressCache
            {
                Id = id
            };
        }

        await _mediaStoreContext.GeoAddressCache.InsertOneAsync(
            cached,
            options: null,
            cancellationToken);

        return geoAddress;
    }
}
