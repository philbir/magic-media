using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;

namespace MagicMedia.Store
{
    public interface IFaceStore
    {
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<MediaFace> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<IEnumerable<PersonEncodingData>> GetPersonEncodingsAsync(
            CancellationToken cancellationToken);

        Task<IEnumerable<MediaFace>> SearchAsync(
            SearchFacesRequest request,
            CancellationToken cancellationToken);
        Task UpdateAsync(MediaFace face, CancellationToken cancellationToken);
    }
}
