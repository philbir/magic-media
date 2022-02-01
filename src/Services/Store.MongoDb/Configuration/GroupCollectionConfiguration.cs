using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class GroupCollectionConfiguration :
    IMongoCollectionConfiguration<Group>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<Group> builder)
    {
        builder
            .WithCollectionName(CollectionNames.Group)
            .AddBsonClassMap<Group>(cm =>
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
