using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration;

internal class MediaExportProfileCollectionConfiguration :
    IMongoCollectionConfiguration<MediaExportProfile>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<MediaExportProfile> builder)
    {
        builder
            .WithCollectionName(CollectionNames.MediaExportProfile)
            .AddBsonClassMap<Group>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            })
            .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
            .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest);
    }
}
