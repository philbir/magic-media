using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo;

internal class SignUpSessionCollectionConfiguration :
    IMongoCollectionConfiguration<SignUpSession>
{
    public void OnConfiguring(
        IMongoCollectionBuilder<SignUpSession> builder)
    {
        builder
            .WithCollectionName("signUpSession")
            .AddBsonClassMap<SignUpSession>(cm =>
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
