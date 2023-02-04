using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Operations;

namespace MagicMedia;

public interface IMediaOperationsService
{
    Task DeleteAsync(DeleteMediaRequest request, CancellationToken cancellationToken);
    Task MoveMediaAsync(
        MoveMediaRequest request,
        CancellationToken cancellationToken);
    Task RecycleAsync(RecycleMediaRequest request, CancellationToken cancellationToken);
    Task ReScanFacesAsync(RescanFacesRequest request, CancellationToken cancellationToken);
    Task ToggleFavoriteAsync(Guid id, bool isFavorite, CancellationToken cancellationToken);
    Task UpdateMetadataAsync(UpdateMediaMetadataRequest request, CancellationToken cancellationToken);
}
