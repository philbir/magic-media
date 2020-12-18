using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.Face
{
    public interface IFaceService
    {
        Task<IEnumerable<MediaFace>> ApproveAllByMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<MediaFace> ApproveComputerAsync(Guid id, CancellationToken cancellationToken);

        Task<MediaFace> AssignPersonByHumanAsync(
            Guid faceId,
            string personName,
            CancellationToken cancellationToken);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> DeleteUnassingedByMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<MediaFace> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<MediaThumbnail> GetThumbnailAsync(Guid id, CancellationToken cancellationToken);
        Task<(MediaFace face, bool hasMatch)> PredictPersonAsync(
            Guid faceId,
            double? distance,
            CancellationToken cancellationToken);
        Task<IEnumerable<(MediaFace face, bool hasMatch)>> PredictPersonsByMediaAsync(Guid mediaId, double? distance, CancellationToken cancellationToken);
        Task<SearchResult<MediaFace>> SearchAsync(SearchFacesRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<MediaFace>> UnassignAllPredictedByMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<MediaFace> UnAssignPersonAsync(Guid id, CancellationToken cancellationToken);
    }
}
