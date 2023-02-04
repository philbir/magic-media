using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class GeoAddressCacheCollectionConfiguration :
    IMongoCollectionConfiguration<GeoAddressCache>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<GeoAddressCache> builder)
    {
        builder
            .WithCollectionName(CollectionNames.GeoAddressCache)
            .AddBsonClassMap<GeoAddressCache>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            })
            .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
            .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
            .WithCollectionConfiguration(collection =>
            {

            });
    }
}
