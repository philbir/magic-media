using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store.MongoDb
{
    public class AuditEventStore : IAuditEventStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public AuditEventStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task AddManyAsync(
            IEnumerable<AuditEvent> auditEvents,
            CancellationToken cancellationToken)
        {
            await _mediaStoreContext.AuditEvents.InsertManyAsync(
                auditEvents,
                DefaultMongoOptions.InsertMany,
                cancellationToken);
        }
    }
}
