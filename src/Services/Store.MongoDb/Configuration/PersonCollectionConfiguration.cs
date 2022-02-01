using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class PersonCollectionConfiguration :
    IMongoCollectionConfiguration<Person>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<Person> builder)
    {
        builder
            .WithCollectionName(CollectionNames.Person)
            .AddBsonClassMap<Person>(cm =>
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
