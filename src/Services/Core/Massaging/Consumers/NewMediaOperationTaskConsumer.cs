using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Messaging
{
    public class NewMediaOperationTaskConsumer : IConsumer<NewMediaOperationTaskMessage>
    {
        private readonly IMediaOperationTaskHandler _handler;

        public NewMediaOperationTaskConsumer(
            IMediaOperationTaskHandler handler)
        {
            _handler = handler;
        }

        public async Task Consume(ConsumeContext<NewMediaOperationTaskMessage> context)
        {
            await _handler.ExecuteAsync(
                context.Message.OperationId,
                context.Message.Task,
                context.CancellationToken);
        }
    }
}
