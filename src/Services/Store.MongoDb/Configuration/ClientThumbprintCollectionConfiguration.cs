using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{
    internal class ClientThumbprintCollectionConfiguration :
        IMongoCollectionConfiguration<ClientThumbprint>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<ClientThumbprint> builder)
        {
            builder
                .WithCollectionName(CollectionNames.ClientThumbprint)
                .AddBsonClassMap<ClientThumbprint>(cm =>
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
}
