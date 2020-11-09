using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Operations;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia
{
    public class MediaOperationsService : IMediaOperationsService
    {
        private readonly IOperationStore _operationStore;
        private readonly IBus _bus;

        public MediaOperationsService(IOperationStore operationStore, IBus bus)
        {
            _operationStore = operationStore;
            _bus = bus;
        }
        public async Task<MediaOperationResult> MoveMediaAsync(
            MoveMediaRequest request,
            CancellationToken cancellationToken)
        {
            MediaOperation operation = BuildOperation(request);

            await _operationStore.AddAsync(operation, cancellationToken);

            await PublishMessagesAsync(operation);

            return new MediaOperationResult
            {
                Id = operation.Id
            };
        }

        private async Task PublishMessagesAsync(MediaOperation operation)
        {
            var tasks = new List<Task>();

            foreach (MediaOperationTask? task in operation.Tasks)
            {
                var message = new NewMediaOperationTaskMessage
                {
                    OperationId = operation.Id,
                    Task = task
                };

                tasks.Add(_bus.Publish(message));
            }

            await Task.WhenAll(tasks);
        }

        private MediaOperation BuildOperation(MoveMediaRequest moveMediaRequest)
        {
            var operation = new MediaOperation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Description = $"Move items to {moveMediaRequest.NewLocation}",
                Name = "MoveMedia",
                Tasks = CreateTasks(moveMediaRequest)
            };

            return operation;
        }

        private IEnumerable<MediaOperationTask> CreateTasks(MoveMediaRequest moveMediaRequest)
        {
            return moveMediaRequest.Ids.Select(id => new MediaOperationTask
            {
                Name = "MoveMedia",
                Entity = new OperationEntityIdentifier
                {
                    Type = "Media",
                    Id = id
                },
                Data = new Dictionary<string, object>
                {
                    { "NewLocation", moveMediaRequest.NewLocation }
                },
                Steps = BuildMoveMediaSteps()
            });
        }

        private IEnumerable<MediaOperationStep> BuildMoveMediaSteps()
        {
            yield return new MediaOperationStep
            {
                Name = MediaOperationStepNames.MoveMediaFile,
                State = MediaOperationStepState.New,
            };

            yield return new MediaOperationStep
            {
                Name = MediaOperationStepNames.MoveMediaUpdateMetadata,
                State = MediaOperationStepState.New,
            };
        }
    }
}
