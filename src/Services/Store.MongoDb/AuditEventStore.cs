using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MongoDB.Driver;

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

        public async Task<SearchResult<AuditEvent>> SearchAsync(
            SearchAuditRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<AuditEvent> filter = Builders<AuditEvent>.Filter.Empty;

            if (request.UserId.HasValue)
            {
                filter = Builders<AuditEvent>.Filter.Eq(x => x.UserId, request.UserId.Value);
            }

            if ( request.Actions is { } a && a.Any())
            {
                filter = Builders<AuditEvent>.Filter.In(x => x.Action, a);
            }

            if (request.Success.HasValue)
            {
                filter = Builders<AuditEvent>.Filter.Eq(x => x.Success, request.Success.Value);
            }

            IFindFluent<AuditEvent, AuditEvent>? cursor = _mediaStoreContext.AuditEvents.Find(filter);
            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<AuditEvent> events = await cursor
                .SortByDescending(x => x.Timestamp)
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new SearchResult<AuditEvent>(events, (int)totalCount);
        }
    }
}
