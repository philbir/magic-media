using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

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

            foreach (MediaOperationStep step in task.Steps)
            {
                var ctx = new MediaOperationStepContext(
                    operationId,
                    step,
                    task,
                    cancellationToken);

                await ExecuteStepAsync(ctx);
            }

            //Save
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

            MediaOperationStepResult result = await instance.ExecuteAsync(context);

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
