using MagicMedia.Operations;
using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{
    internal class MediaOperationCollectionConfiguration :
        IMongoCollectionConfiguration<MediaOperation>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<MediaOperation> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Camera)
                .AddBsonClassMap<MediaOperation>(cm =>
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
