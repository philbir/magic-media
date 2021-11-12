using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Thumbprint;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class NewAuditEventBatchConsumer : IConsumer<Batch<NewAuditEventMessage>>
    {
        private readonly IAuditEventStore _eventStore;
        private readonly IClientThumbprintService _thumbprintService;

        public NewAuditEventBatchConsumer(
            IAuditEventStore eventStore,
            IClientThumbprintService thumbprintService)
        {
            _eventStore = eventStore;
            _thumbprintService = thumbprintService;
        }

        public async Task Consume(ConsumeContext<Batch<NewAuditEventMessage>> context)
        {
            IEnumerable<AuditEvent> events = context.Message.Select(x => x.Message.Event);

            await AddClientThumbprintAsync(events, context.CancellationToken);

            await _eventStore.AddManyAsync(events, context.CancellationToken);
        }

        private async Task AddClientThumbprintAsync(
            IEnumerable<AuditEvent> events,
            CancellationToken cancellationToken)
        {
            foreach (AuditEvent auditEvent in events)
            {
                string id = await _thumbprintService.GetOrCreateAsync(
                    auditEvent.Client,
                    cancellationToken);

                auditEvent.ThumbprintId = id;
                auditEvent.Client.IPAdddress = null;
                auditEvent.Client.UserAgent = null;
            }
        }
    }
}
