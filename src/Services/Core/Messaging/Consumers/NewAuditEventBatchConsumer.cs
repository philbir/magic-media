using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class NewAuditEventBatchConsumer : IConsumer<Batch<NewAuditEventMessage>>
    {
        private readonly IAuditEventStore _eventStore;

        public NewAuditEventBatchConsumer(IAuditEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task Consume(ConsumeContext<Batch<NewAuditEventMessage>> context)
        {
            IEnumerable<AuditEvent> events = context.Message.Select(x => x.Message.Event);

            await _eventStore.AddManyAsync(events, context.CancellationToken);
        }
    }
}
