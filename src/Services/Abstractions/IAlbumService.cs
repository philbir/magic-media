using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia;

public interface IAlbumService
{
    Task<Album> AddAsync(string title, CancellationToken cancellationToken);
    Task<Album> AddItemsToAlbumAsync(
        AddItemToAlbumRequest request,
        CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken);
    Task<Album> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Album>> GetSharedByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<MediaThumbnail?> GetThumbnailAsync(
        Album album,
        ThumbnailSizeName size,
        CancellationToken cancellationToken);
    Task<IEnumerable<Album>> GetWithPersonAsync(Guid personId, CancellationToken cancellationToken);
    Task<Album> RemoveFoldersAsync(RemoveFoldersFromAlbumRequest request, CancellationToken cancellationToken);
    Task SaveUserSharedAlbumsAsync(SaveUserSharedAlbumsRequest request, CancellationToken cancellationToken);
    Task<SearchResult<Album>> SearchAsync(
        SearchAlbumRequest request,
        CancellationToken cancellationToken);
    Task<Album> SetCoverAsync(SetAlbumCoverRequest request, CancellationToken cancellationToken);
    Task<Album> UpdateAlbumAsync(
        UpdateAlbumRequest request,
        CancellationToken cancellationToken);
}
