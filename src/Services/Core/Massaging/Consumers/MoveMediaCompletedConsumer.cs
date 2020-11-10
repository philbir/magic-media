using System.Linq;
using System.Threading.Tasks;
using MagicMedia.Hubs;
using MagicMedia.Messaging;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace MagicMedia.Massaging.Consumers
{
    public class MoveMediaCompletedConsumer : IConsumer<MoveMediaCompletedMessage>
    {
        private readonly IHubContext<MediaHub> _hubContext;

        public MoveMediaCompletedConsumer(IHubContext<MediaHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<MoveMediaCompletedMessage> context)
        {
            await _hubContext.Clients.All.SendAsync(
                "moveMediaCompleted",
                context.Message,
                context.CancellationToken);
        }
    }

    public class MoveMediaRequestCompletedConsumer : IConsumer<MoveMediaRequestCompletedMessage>
    {
        private readonly IHubContext<MediaHub> _hubContext;

        public MoveMediaRequestCompletedConsumer(IHubContext<MediaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<MoveMediaRequestCompletedMessage> context)
        {
            await _hubContext.Clients.All.SendAsync(
                "moveMediaRequestCompleted",
                context.Message,
                context.CancellationToken);
        }
    }
}
