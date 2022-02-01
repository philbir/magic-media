using System.Threading.Tasks;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class FaceUpdatedConsumer : IConsumer<FaceUpdatedMessage>
{
    public Task Consume(ConsumeContext<FaceUpdatedMessage> context)
    {
        return Task.CompletedTask;
    }
}
