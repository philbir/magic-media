using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class TagDefinitionCollectionConfiguration :
    IMongoCollectionConfiguration<TagDefintion>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<TagDefintion> builder)
    {
        builder
            .WithCollectionName(CollectionNames.TagDefinition)
            .AddBsonClassMap<Group>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            })
            .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
            .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest);
    }
}
