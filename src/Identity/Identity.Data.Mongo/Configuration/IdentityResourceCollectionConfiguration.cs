using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo
{
    internal class IdentityResourceCollectionConfiguration :
        IMongoCollectionConfiguration<MagicIdentityResource>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<MagicIdentityResource> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .WithCollectionName("identityResource")
                .AddBsonClassMap<MagicIdentityResource>(cm =>
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
