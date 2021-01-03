using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;
using MassTransit;
using Serilog;

namespace MagicMedia.Audit
{
    public class AuditService : IAuditService
    {
        private readonly IAuditEventStore _auditEventStore;
        private readonly IBus _bus;
        private readonly IUserContextFactory _userContextFactory;

        public AuditService(
            IAuditEventStore auditEventStore,
            IBus bus,
            IUserContextFactory userContextFactory)
        {
            _auditEventStore = auditEventStore;
            _bus = bus;
            _userContextFactory = userContextFactory;
        }

        public Task LogEventAsync(
            LogAuditEventRequest request,
            IUserContext userContext,
            CancellationToken cancellationToken)
        {
            var auditEvent = new AuditEvent
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Client = userContext.GetClientInfo(),
                UserId = userContext.UserId,
                Resource = request.Resource,
                Success = request.Success,
                Action = request.Action,
                Hostname = Environment.MachineName,
                GrantFrom = request.GrantBy
            };

            if (auditEvent.Client.IPAdddress.IsInternalIP())
            {
                return Task.CompletedTask;
            }

            try
            {
                Task.Run(() => SendEventAsync(auditEvent, cancellationToken)).Forget();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not send audit event");
            }

            return Task.CompletedTask;
        }

        public async Task LogEventAsync(LogAuditEventRequest request, CancellationToken cancellationToken)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            await LogEventAsync(request, userContext, cancellationToken);
        }

        private async Task SendEventAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
        {
            await _bus.Publish(new NewAuditEventMessage(auditEvent), cancellationToken);
        }

        public async Task<SearchResult<AuditEvent>> SearchAsync(
            SearchAuditRequest request,
            CancellationToken cancellationToken)
        {
            return await _auditEventStore.SearchAsync(request, cancellationToken);
        }
    }
}
