using System.Threading.Tasks;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class FavoriteMediaToggledConsumer : IConsumer<FavoriteMediaToggledMessage>
{
    public Task Consume(ConsumeContext<FavoriteMediaToggledMessage> context)
    {
        //Update Album...

        return Task.CompletedTask;
    }
}
