using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Search;

namespace MagicMedia.Store
{
    public interface IFaceStore
    {
        Task BulkUpdateAgesAsync(IEnumerable<UpdateAgeRequest> updates, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<MediaFace> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken);

        Task<IEnumerable<MediaFace>> GetManyByMediaAsync(
            IEnumerable<Guid> mediaIds,
            CancellationToken cancellationToken);

        Task<IEnumerable<MediaFace>> GetFacesByPersonAsync(
            Guid personId,
            IEnumerable<Guid>? faceIds,
            CancellationToken cancellationToken);

        Task<IEnumerable<PersonEncodingData>> GetPersonEncodingsAsync(
            CancellationToken cancellationToken);

        Task<SearchResult<MediaFace>> SearchAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken);

        Task UpdateAsync(MediaFace face, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetPersonsIdsByMediaAsync(IEnumerable<Guid> mediaIds, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetIdsByMediaAsync(IEnumerable<Guid> mediaIds, CancellationToken cancellationToken);
    }
}
