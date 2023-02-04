using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo;

internal class OneLoginClientCollectionConfiguration
    : IMongoCollectionConfiguration<MagicClient>
{
    public void OnConfiguring(IMongoCollectionBuilder<MagicClient> mongoCollectionBuilder)
    {
        mongoCollectionBuilder
            .WithCollectionName("client")
            .AddBsonClassMap<MagicClient>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            })
            .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
            .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
            .WithCollectionConfiguration(collection =>
            {
                var extIdIndex = new CreateIndexModel<MagicClient>(
                    Builders<MagicClient>.IndexKeys.Ascending(c => c.ClientId),
                    new CreateIndexOptions { Unique = true });
                collection.Indexes.CreateOne(extIdIndex);
            });
    }
}
