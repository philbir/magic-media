using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class SimilarMediaInfoCollectionConfiguration :
    IMongoCollectionConfiguration<SimilarMediaInfo>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<SimilarMediaInfo> builder)
    {
        builder
            .WithCollectionName(CollectionNames.SimilarMedia)
            .AddBsonClassMap<SimilarMediaInfo>(cm =>
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
