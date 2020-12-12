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
                    var mediaIdIndex = new CreateIndexModel<MediaAI>(
                         Builders<MediaAI>.IndexKeys
                             .Ascending(c => c.MediaId),
                         new CreateIndexOptions { Unique = true });

                    collection.Indexes.CreateOne(mediaIdIndex);

                    var objectNameIndex = new CreateIndexModel<MediaAI>(
                         Builders<MediaAI>.IndexKeys
                             .Ascending("Objects.Name"),
                         new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(objectNameIndex);

                    var tagNameIndex = new CreateIndexModel<MediaAI>(
                     Builders<MediaAI>.IndexKeys
                         .Ascending("Tags.Name"),
                     new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(tagNameIndex);
                });
        }
    }
}
