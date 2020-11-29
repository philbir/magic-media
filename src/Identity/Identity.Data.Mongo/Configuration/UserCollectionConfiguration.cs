using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo
{
    internal class UserCollectionConfiguration :
        IMongoCollectionConfiguration<User>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<User> builder)
        {
            builder
                .WithCollectionName("user")
                .AddBsonClassMap<User>(cm =>
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
