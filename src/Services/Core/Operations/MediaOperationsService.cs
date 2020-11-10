using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MassTransit;

namespace MagicMedia.Operations
{
    public class MediaOperationsService : IMediaOperationsService
    {
        private readonly IBus _bus;

        public MediaOperationsService(IBus bus)
        {
            _bus = bus;
        }
        public async Task MoveMediaAsync(
            MoveMediaRequest request,
            CancellationToken cancellationToken)
        {
            var message = new MoveMediaMessage
            {
                Ids = request.Ids,
                NewLocation = request.NewLocation,
                OperationId = request.OperationId!,
            };

            await _bus.Publish(message, cancellationToken);
        }
    }
}
