using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo;

internal class ApiResourceCollectionConfiguration :
    IMongoCollectionConfiguration<MagicApiResource>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<MagicApiResource> mongoCollectionBuilder)
    {
        mongoCollectionBuilder
            .WithCollectionName("apiResource")
            .AddBsonClassMap<MagicApiResource>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            })
            .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
            .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
            .WithCollectionConfiguration(collection =>
            {
                var nameIndex = new CreateIndexModel<MagicApiResource>(
                    Builders<MagicApiResource>.IndexKeys.Ascending(c => c.Name),
                    new CreateIndexOptions { Unique = true });
                collection.Indexes.CreateOne(nameIndex);
            });
    }
}
