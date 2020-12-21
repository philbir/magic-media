using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public interface IAuditEventStore
    {
        Task AddManyAsync(IEnumerable<AuditEvent> auditEvents, CancellationToken cancellationToken);
    }
}
