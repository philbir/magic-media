using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;
using SixLabors.ImageSharp.ColorSpaces;

namespace MagicMedia.Operations
{
    public class MediaOperationTaskHandler : IMediaOperationTaskHandler
    {
        private readonly IEnumerable<IMediaOperationStep> _steps;
        private readonly IOperationStore _operationStore;
        private readonly IBus _bus;

        public MediaOperationTaskHandler(
            IEnumerable<IMediaOperationStep> steps,
            IOperationStore operationStore,
            IBus bus)
        {
            _steps = steps;
            _operationStore = operationStore;
            _bus = bus;
        }

        public async Task ExecuteAsync(
            Guid operationId,
            MediaOperationTask task,
            CancellationToken cancellationToken)
        {
            MediaOperation operation = await _operationStore.GetAsync(
                operationId,
                cancellationToken);

            MediaOperationTask savedTask = operation.Tasks.Single(x => x.Id == task.Id);

            foreach (MediaOperationStep step in savedTask.Steps.Where(x => x.State == MediaOperationStepState.New))
            {
                var ctx = new MediaOperationStepContext(
                    operationId,
                    step,
                    task,
                    cancellationToken);

                await ExecuteStepAsync(ctx);
            }
        }

        private async Task ExecuteStepAsync(
            MediaOperationStepContext context)
        {
            IMediaOperationStep? instance = _steps
                .FirstOrDefault(x => x.Name == context.Step.Name);

            if (instance == null)
            {
                throw new ApplicationException(
                    $"No OperationStep with name: { context.Step.Name} found.");
            }

            MediaOperationStep storedStep = context.Task.Steps.Single(x => x.Name == context.Step.Name);

            MediaOperationStepResult? result = null;

            try
            {
                result = await instance.ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                result = new MediaOperationStepResult
                {
                    State = MediaOperationStepState.Failed,
                    Messages = new List<string> { ex.Message }
                };
            }

            storedStep.State = MediaOperationStepState.Failed;
            var messages = new List<string>(storedStep.Messages);
            messages.AddRange(result.Messages);
            storedStep.Messages = messages;

            await _operationStore.UpdateTaskAsync(context.OperationId, context.Task, context.OperationAbord);

            var message = new MediaOperationStepExecutedMessage
            {
                OperationId = context.OperationId,
                Result = result,
                Step = context.Step
            };

            await _bus.Publish(message, context.OperationAbord);
        }
    }
}
