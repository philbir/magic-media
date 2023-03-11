using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class FaceUpdatedConsumer : IConsumer<FaceUpdatedMessage>
{
    private readonly IFaceService _faceService;

    public FaceUpdatedConsumer(IFaceService faceService)
    {
        _faceService = faceService;
    }

    public async Task Consume(ConsumeContext<FaceUpdatedMessage> context)
    {
        MediaFace face = await _faceService.GetByIdAsync(context.Message.Id, context.CancellationToken);

        await _faceService.UpdateAgeAsync(face, context.CancellationToken);
    }
}
