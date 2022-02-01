using System.Threading.Tasks;
using MagicMedia.Processing;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class NewMediaAddedConsumer : IConsumer<NewMediaAddedMessage>
{
    private readonly IMediaFaceScanner _mediaFaceScanner;
    private readonly IMediaService _mediaService;
    private readonly IMediaProcessorFlow _videoFlow;

    public NewMediaAddedConsumer(
        IMediaFaceScanner mediaFaceScanner,
        IMediaService mediaService,
        IMediaProcessorFlowFactory flowFactory)
    {
        _mediaFaceScanner = mediaFaceScanner;
        _mediaService = mediaService;
        _videoFlow = flowFactory.CreateFlow("BuildPreviewVideos");
    }

    public async Task Consume(ConsumeContext<NewMediaAddedMessage> context)
    {
        Media media = await _mediaService.GetByIdAsync(
            context.Message.Id,
            context.CancellationToken);

        if (media.MediaType == MediaType.Image)
        {
            await _mediaFaceScanner.ScanByMediaIdAsync(
                context.Message.Id,
                context.CancellationToken);
        }

        if (media.MediaType == MediaType.Video)
        {
            await _videoFlow.ExecuteAsync(new MediaProcessorContext
            {
                Media = media
            }, context.CancellationToken);
        }
    }
}
