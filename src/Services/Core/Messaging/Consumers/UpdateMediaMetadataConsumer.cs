using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class UpdateMediaMetadataConsumer : IConsumer<UpdateMediaMetadataMessage>
    {
        private readonly IUpdateMediaMetadataHandler _metadataHandler;

        public UpdateMediaMetadataConsumer(IUpdateMediaMetadataHandler metadataHandler)
        {
            _metadataHandler = metadataHandler;
        }

        public async Task Consume(ConsumeContext<UpdateMediaMetadataMessage> context)
        {
            await _metadataHandler.ExecuteAsync(
                context.Message,
                context.CancellationToken);
        }
    }
}
