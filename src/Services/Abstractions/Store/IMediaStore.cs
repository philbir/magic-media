using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store
{
    public interface IMediaStore
    {
        IFaceStore Faces { get; }

        Task<IEnumerable<string>> GetAllFoldersAsync(CancellationToken cancellationToken);
        Task<Media> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<SearchFacetItem>> GetGroupedCitiesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SearchFacetItem>> GetGroupedCountriesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Media>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<IReadOnlyDictionary<Guid, MediaThumbnail>> GetThumbnailsByMediaIdsAsync(
            IEnumerable<Guid> mediaIds,
            ThumbnailSizeName size,
            CancellationToken cancellationToken);

        Task InsertMediaAsync(
            Media media,
            IEnumerable<MediaFace> faces,
            CancellationToken cancellationToken);
        Task SaveFacesAsync(Guid mediaId, IEnumerable<MediaFace> faces, CancellationToken cancellationToken);

        Task<SearchResult<Media>> SearchAsync(SearchMediaRequest request, CancellationToken cancellationToken);
        Task UpdateAsync(Media media, CancellationToken cancellationToken);
    }
}
