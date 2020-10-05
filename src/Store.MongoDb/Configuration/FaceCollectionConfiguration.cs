using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{
    internal class FaceCollectionConfiguration :
        IMongoCollectionConfiguration<MediaFace>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<MediaFace> builder)
        {
            builder
                .WithCollectionName(CollectionNames.Face)
                .AddBsonClassMap<MediaFace>(cm =>
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
