using System.Threading.Tasks;
using MagicMedia.Processing;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class NewMediaAddedConsumer : IConsumer<NewMediaAddedMessage>
    {
        private readonly IMediaFaceScanner _mediaFaceScanner;

        public NewMediaAddedConsumer(IMediaFaceScanner mediaFaceScanner)
        {
            _mediaFaceScanner = mediaFaceScanner;
        }

        public async Task Consume(ConsumeContext<NewMediaAddedMessage> context)
        {
            await _mediaFaceScanner.ScanByMediaIdAsync(context.Message.Id, context.CancellationToken);
        }
    }
}
