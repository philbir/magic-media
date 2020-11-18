using System.Linq;
using System.Threading.Tasks;
using MagicMedia.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace MagicMedia.Messaging.Consumers
{
    public class MoveMediaCompletedConsumer : IConsumer<MediaOperationCompletedMessage>
    {
        private readonly IHubContext<MediaHub> _hubContext;

        public MoveMediaCompletedConsumer(IHubContext<MediaHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<MediaOperationCompletedMessage> context)
        {
            await _hubContext.Clients.All.SendAsync(
                "moveMediaCompleted",
                context.Message,
                context.CancellationToken);
        }
    }

    public class MoveMediaRequestCompletedConsumer : IConsumer<MediaOperationRequestCompletedMessage>
    {
        private readonly IHubContext<MediaHub> _hubContext;

        public MoveMediaRequestCompletedConsumer(IHubContext<MediaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<MediaOperationRequestCompletedMessage> context)
        {
            await _hubContext.Clients.All.SendAsync(
                "moveMediaRequestCompleted",
                context.Message,
                context.CancellationToken);
        }
    }
}
