using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store.MongoDb;

namespace MagicMedia.Store
{
    public interface IMediaStore
    {
        IFaceStore Faces { get; }

        IPersonStore Persons { get; }

        IAlbumStore Albums { get; }

        IThumbnailBlobStore Thumbnails { get; }
        IMediaBlobStore Blob { get; }
        IMediaAIStore MediaAI { get; }

        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<MediaGeoLocation>> FindMediaInGeoBoxAsync(
            GeoBox box,
            int limit,
            CancellationToken cancellation);

        Task<IEnumerable<string>> GetAllFoldersAsync(CancellationToken cancellationToken);
        Task<Media> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<SearchFacetItem>> GetGroupedCitiesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SearchFacetItem>> GetGroupedCountriesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MediaHeaderData>> GetHeaderDataAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetIdsByFolderAsync(string folder, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetIdsFromSearchRequestAsync(SearchMediaRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<Media>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<IEnumerable<Media>> GetMediaWithoutAISourceAsync(AISource source, int limit, CancellationToken cancellationToken);
        Task<IReadOnlyDictionary<Guid, MediaThumbnail>> GetThumbnailsByMediaIdsAsync(
            IEnumerable<Guid> mediaIds,
            ThumbnailSizeName size,
            CancellationToken cancellationToken);

        Task InsertMediaAsync(
            Media media,
            IEnumerable<MediaFace>? faces,
            CancellationToken cancellationToken);
        Task SaveFacesAsync(Guid mediaId, IEnumerable<MediaFace> faces, CancellationToken cancellationToken);

        Task<SearchResult<Media>> SearchAsync(
            SearchMediaRequest request,
            Func<Guid, CancellationToken, Task<IEnumerable<Guid>>> albumMediaResolver,
            CancellationToken cancellationToken);
        Task UpdateAISummaryAsync(Guid mediaId, MediaAISummary mediaAISummary, CancellationToken cancellationToken);
        Task UpdateAsync(Media media, CancellationToken cancellationToken);
    }
}
