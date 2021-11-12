using MagicMedia.Audit;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class AuditQueries
    {
        private readonly IAuditService _auditService;

        public AuditQueries(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<SearchResult<AuditEvent>> SearchAuditEventsAsync(
            SearchAuditRequest request,
            CancellationToken cancellationToken)
        {
            return await _auditService.SearchAsync(request, cancellationToken);
        }
    }
}
