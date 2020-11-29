using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class MoveMediaConsumer : IConsumer<MoveMediaMessage>
    {
        private readonly IMoveMediaHandler _moveMediaHandler;

        public MoveMediaConsumer(IMoveMediaHandler moveMediaHandler)
        {
            _moveMediaHandler = moveMediaHandler;
        }

        public async Task Consume(ConsumeContext<MoveMediaMessage> context)
        {
            await _moveMediaHandler.ExecuteAsync(
                context.Message,
                context.CancellationToken);
        }
    }
}
