using System.Threading.Tasks;
using MagicMedia.Operations;
using MagicMedia.Processing;
using MagicMedia.Store;
using MassTransit;
using SixLabors.ImageSharp;

namespace MagicMedia.Messaging.Consumers;

public class DeleteMediaConsumer : IConsumer<DeleteMediaMessage>
{
    private readonly IDeleteMediaHandler _deleteMediaHandler;

    public DeleteMediaConsumer(IDeleteMediaHandler deleteMediaHandler)
    {
        _deleteMediaHandler = deleteMediaHandler;
    }

    public async Task Consume(ConsumeContext<DeleteMediaMessage> context)
    {
        await _deleteMediaHandler.ExecuteAsync(
            context.Message,
            context.CancellationToken);
    }
}

public class MediaEditedConsumer : IConsumer<MediaEditedMessage>
{
    private readonly IMediaService _mediaService;
    private readonly IMediaProcessorFlowFactory _flowFactory;

    public MediaEditedConsumer(
        IMediaService mediaService,
        IMediaProcessorFlowFactory flowFactory)
    {
        _mediaService = mediaService;
        _flowFactory = flowFactory;
    }
    public async Task Consume(ConsumeContext<MediaEditedMessage> context)
    {
        IMediaProcessorFlow flow = _flowFactory.CreateFlow("ScanFaces");
        Media media = await _mediaService.GetByIdAsync(context.Message.Id, context.CancellationToken);

        var fileName = _mediaService.GetFilename(media, MediaFileType.Original);
        Image? image = await Image.LoadAsync(fileName);

        var processorContext = new MediaProcessorContext { Media = media, Image = image };

        await flow.ExecuteAsync(processorContext, context.CancellationToken);
    }
}
