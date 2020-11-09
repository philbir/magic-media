using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Operations;

namespace MagicMedia
{
    public interface IMediaOperationsService
    {
        Task<MediaOperationResult> MoveMediaAsync(
            MoveMediaRequest request,
            CancellationToken cancellationToken);
    }
}
