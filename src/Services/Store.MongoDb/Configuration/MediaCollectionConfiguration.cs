using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{

    internal class MediaCollectionConfiguration :
        IMongoCollectionConfiguration<Media>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Media> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Media)
                .AddBsonClassMap<Media>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .AddBsonClassMap<MediaThumbnail>(cm =>
                {
                    cm.AutoMap();
                    cm.UnmapMember(x => x.Data);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {

                });
        }
    }
}
