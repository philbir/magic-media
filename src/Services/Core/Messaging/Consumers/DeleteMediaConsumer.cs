using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

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
