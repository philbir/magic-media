using System;
using System.Threading;
using System.Threading.Tasks;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public class MagicMediaTest
        : IMagicMediaTest
    {
        private const string _clientName = "MagicMediaTest";

        private readonly IOperationExecutor _executor;

        public MagicMediaTest(IOperationExecutorPool executorPool)
        {
            _executor = executorPool.CreateExecutor(_clientName);
        }

        public Task<IOperationResult<IMediaDetails>> MediaDetailsAsync(
            Optional<System.Guid> id = default,
            CancellationToken cancellationToken = default)
        {

            return _executor.ExecuteAsync(
                new MediaDetailsOperation { Id = id },
                cancellationToken);
        }

        public Task<IOperationResult<IMediaDetails>> MediaDetailsAsync(
            MediaDetailsOperation operation,
            CancellationToken cancellationToken = default)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            return _executor.ExecuteAsync(operation, cancellationToken);
        }
    }
}
