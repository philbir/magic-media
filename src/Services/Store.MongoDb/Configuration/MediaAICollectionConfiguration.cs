using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{
    internal class MediaAICollectionConfiguration :
        IMongoCollectionConfiguration<MediaAI>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<MediaAI> builder)
        {
            builder
                .WithCollectionName(CollectionNames.MediaAI)
                .AddBsonClassMap<MediaAI>(cm =>
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
