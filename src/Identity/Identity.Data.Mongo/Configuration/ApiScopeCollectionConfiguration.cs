using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo;

internal class ApiScopeCollectionConfiguration
    : IMongoCollectionConfiguration<MagicApiScope>
{
    public void OnConfiguring(IMongoCollectionBuilder<MagicApiScope> mongoCollectionBuilder)
    {
        mongoCollectionBuilder
            .WithCollectionName("apiScope")
            .AddBsonClassMap<MagicApiScope>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            })
            .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
            .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
            .WithCollectionConfiguration(collection =>
            {
                var nameIndex = new CreateIndexModel<MagicApiScope>(
                    Builders<MagicApiScope>.IndexKeys.Ascending(c => c.Name),
                    new CreateIndexOptions { Unique = true });

                collection.Indexes.CreateOne(nameIndex);
            });
    }
}
