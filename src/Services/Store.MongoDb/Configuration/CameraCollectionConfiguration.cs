using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class CameraCollectionConfiguration :
    IMongoCollectionConfiguration<Camera>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<Camera> builder)
    {
        builder
            .WithCollectionName(CollectionNames.Camera)
            .AddBsonClassMap<Camera>(cm =>
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
