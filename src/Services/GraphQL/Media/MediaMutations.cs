using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using MagicMedia.Messaging;
using MagicMedia.Operations;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    public class MediaMutations
    {
        private readonly IMediaOperationsService _operationsService;

        public MediaMutations(IMediaOperationsService operationsService)
        {
            _operationsService = operationsService;
        }

        public async Task<MediaOperationPayload> MoveMediaAsync(MoveMediaRequest request, [Service] ITopicEventSender eventSender, CancellationToken cancellationToken)
        {
            MediaOperationResult operation = await _operationsService.MoveMediaAsync(request, cancellationToken);

            await eventSender.SendAsync("MediaOperation", new NewMediaOperationTaskMessage
            {
                OperationId = operation.Id,
                Task = new MediaOperationTask
                {
                    Name = "Bla"
                }
            });

            return new MediaOperationPayload(operation.Id);
        }
    }

    [ExtendObjectType(Name = "Subscription")]
    public class MediaSubscriptions
    {
        [Subscribe]
        public NewMediaOperationTaskMessage OperationCompleted([Topic] string name, [EventMessage] NewMediaOperationTaskMessage payload)
        {
            return payload;
        }
    }
}
