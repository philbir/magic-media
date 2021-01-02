using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Audit;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
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
