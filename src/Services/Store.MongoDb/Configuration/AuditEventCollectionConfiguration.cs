using MongoDB.Driver;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb.Configuration
{
    internal class AuditEventCollectionConfiguration :
        IMongoCollectionConfiguration<AuditEvent>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<AuditEvent> builder)
        {
            builder
                .WithCollectionName(CollectionNames.AuditEvent)
                .AddBsonClassMap<AuditEvent>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                })
                .WithCollectionSettings(s => s.ReadConcern = ReadConcern.Majority)
                .WithCollectionSettings(s => s.ReadPreference = ReadPreference.Nearest)
                .WithCollectionConfiguration(collection =>
                {
                    var userIdIndex = new CreateIndexModel<AuditEvent>(
                         Builders<AuditEvent>.IndexKeys
                             .Ascending(c => c.UserId)
                             .Descending(c => c.Timestamp),
                         new CreateIndexOptions { Unique = false });
                    collection.Indexes.CreateOne(userIdIndex);


                    var actionIndex = new CreateIndexModel<AuditEvent>(
                         Builders<AuditEvent>.IndexKeys
                             .Ascending(c => c.Action)
                             .Descending(c => c.Timestamp),
                         new CreateIndexOptions { Unique = false });
                    collection.Indexes.CreateOne(actionIndex);
                });
        }
    }
}
