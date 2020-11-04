using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store
{
    public interface IMediaStore
    {
        IFaceStore Faces { get; }

        Task<Media> GetById(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyDictionary<Guid, MediaThumbnail>> GetThumbnailsByMediaIdsAsync(
            IEnumerable<Guid> mediaIds,
            ThumbnailSizeName size,
            CancellationToken cancellationToken);

        Task InsertMediaAsync(
            Media media,
            IEnumerable<MediaFace> faces,
            CancellationToken cancellationToken);
        Task SaveFacesAsync(Guid mediaId, IEnumerable<MediaFace> faces, CancellationToken cancellationToken);
        Task<IEnumerable<Media>> SearchAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken);
    }
}
