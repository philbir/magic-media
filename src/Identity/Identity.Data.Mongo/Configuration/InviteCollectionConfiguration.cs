using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Identity.Data.Mongo
{
    internal class InviteCollectionConfiguration :
        IMongoCollectionConfiguration<Invite>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Invite> builder)
        {
            builder
                .WithCollectionName("invite")
                .AddBsonClassMap<Invite>(cm =>
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
