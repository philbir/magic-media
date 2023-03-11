using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Messaging.Consumers;

public class ExportMediaConsumer : IConsumer<ExportMediaMessage>
{
    private readonly IExportMediaHandler _handler;

    public ExportMediaConsumer(IExportMediaHandler handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<ExportMediaMessage> context)
    {
        await _handler.ExecuteAsync(
            context.Message,
            context.CancellationToken);
    }
}
