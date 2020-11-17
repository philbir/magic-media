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
                .AddBsonClassMap<GeoPoint>(cm =>
                {
                    cm.MapMember(x => x.Type)
                        .SetElementName("type");

                    cm.MapMember(x => x.Coordinates)
                        .SetElementName("coordinates");
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(async collection =>
                {
                    var geoIndex = new CreateIndexModel<Media>(
                        Builders<Media>.IndexKeys
                            .Geo2DSphere(x => x.GeoLocation.Point),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(geoIndex);

                    var folderIndex = new CreateIndexModel<Media>(
                        Builders<Media>.IndexKeys
                            .Ascending(c => c.Folder)
                            .Descending(c => c.DateTaken),
                        new CreateIndexOptions { Unique = false });

                    collection.Indexes.CreateOne(folderIndex);
                });
        }
    }
}
