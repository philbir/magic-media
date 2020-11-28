using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class RecycleMediaConsumer : IConsumer<RecycleMediaMessage>
    {
        private readonly IRecycleMediaHandler _recycleMediaHandler;

        public RecycleMediaConsumer(IRecycleMediaHandler recycleMediaHandler)
        {
            _recycleMediaHandler = recycleMediaHandler;
        }

        public async Task Consume(ConsumeContext<RecycleMediaMessage> context)
        {
            await _recycleMediaHandler.ExecuteAsync(
                context.Message,
                context.CancellationToken);
        }
    }


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
}
