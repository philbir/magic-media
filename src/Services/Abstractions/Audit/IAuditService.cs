using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.Audit
{
    public interface IAuditService
    {
        Task LogEventAsync(LogAuditEventRequest request, IUserContext userContext, CancellationToken cancellationToken);
        Task LogEventAsync(LogAuditEventRequest request, CancellationToken cancellationToken);
        Task<SearchResult<AuditEvent>> SearchAsync(SearchAuditRequest request, CancellationToken cancellationToken);
    }

    public class LogAuditEventRequest
    {
        public AuditResource Resource { get; set; }

        public string GrantBy { get; set; }

        public bool Success { get; set; }
        public string Action { get; set; }
    }
}
