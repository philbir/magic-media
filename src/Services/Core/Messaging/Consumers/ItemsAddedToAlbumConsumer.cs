using System.Threading.Tasks;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class ItemsAddedToAlbumConsumer : IConsumer<ItemsAddedToAlbumMessage>
{
    private readonly IAlbumSummaryService _albumSummaryService;

    public ItemsAddedToAlbumConsumer(IAlbumSummaryService albumSummaryService)
    {
        _albumSummaryService = albumSummaryService;
    }

    public async Task Consume(ConsumeContext<ItemsAddedToAlbumMessage> context)
    {
        await _albumSummaryService.UpdateAsync(
            context.Message.Id,
            context.CancellationToken);
    }
}
