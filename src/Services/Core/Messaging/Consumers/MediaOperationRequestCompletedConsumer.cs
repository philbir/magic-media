using System.Threading.Tasks;
using MagicMedia.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace MagicMedia.Messaging.Consumers;

public class MediaOperationRequestCompletedConsumer : IConsumer<MediaOperationRequestCompletedMessage>
{
    private readonly IHubContext<MediaHub> _hubContext;

    public MediaOperationRequestCompletedConsumer(IHubContext<MediaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<MediaOperationRequestCompletedMessage> context)
    {
        await _hubContext.Clients.All.SendAsync(
            "mediaOperationRequestCompleted",
            context.Message,
            context.CancellationToken);
    }
}
