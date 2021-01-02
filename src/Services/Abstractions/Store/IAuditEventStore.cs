using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store
{
    public interface IAuditEventStore
    {
        Task AddManyAsync(IEnumerable<AuditEvent> auditEvents, CancellationToken cancellationToken);
        Task<SearchResult<AuditEvent>> SearchAsync(SearchAuditRequest request, CancellationToken cancellationToken);
    }
}
