using System.Threading;
using System.Threading.Tasks;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public interface IMagicMediaTest
    {
        Task<IOperationResult<IMediaDetails>> MediaDetailsAsync(
            Optional<System.Guid> id = default,
            CancellationToken cancellationToken = default);

        Task<IOperationResult<IMediaDetails>> MediaDetailsAsync(
            MediaDetailsOperation operation,
            CancellationToken cancellationToken = default);
    }
}
