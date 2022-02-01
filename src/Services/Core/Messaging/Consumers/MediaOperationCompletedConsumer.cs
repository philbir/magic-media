using System.Threading.Tasks;
using MagicMedia.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace MagicMedia.Messaging.Consumers;

public class MediaOperationCompletedConsumer : IConsumer<MediaOperationCompletedMessage>
{
    private readonly IHubContext<MediaHub> _hubContext;

    public MediaOperationCompletedConsumer(IHubContext<MediaHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task Consume(ConsumeContext<MediaOperationCompletedMessage> context)
    {
        await _hubContext.Clients.All.SendAsync(
            "mediaOperationCompleted",
            context.Message,
            context.CancellationToken);
    }
}
