using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Audit;

public class AuditService(
    IAuditEventStore auditEventStore,
    IBus bus,
    IUserContextFactory userContextFactory,
    ILogger<AuditService> logger)
    : IAuditService
{
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
            logger.ErrorSendingAuditEvent(ex);
        }

        return Task.CompletedTask;
    }

    public async Task LogEventAsync(LogAuditEventRequest request, CancellationToken cancellationToken)
    {
        IUserContext userContext = await userContextFactory.CreateAsync(cancellationToken);

        await LogEventAsync(request, userContext, cancellationToken);
    }

    private async Task SendEventAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        await bus.Publish(new NewAuditEventMessage(auditEvent), cancellationToken);
    }

    public async Task<SearchResult<AuditEvent>> SearchAsync(
        SearchAuditRequest request,
        CancellationToken cancellationToken)
    {
        return await auditEventStore.SearchAsync(request, cancellationToken);
    }
}

public static partial class AuditServiceLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Could not send audit event. {Ex}")]
    public static partial void ErrorSendingAuditEvent(this ILogger logger, Exception ex);
}
