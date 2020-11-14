using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IAlbumService
    {
        Task<Album> AddAsync(string title, CancellationToken cancellationToken);
        Task<Album> AddItemsToAlbumAsync(AddItemToAlbumRequest request, CancellationToken cancellationToken);
        Task<Album> GetById(Guid id, CancellationToken cancellationToken);
    }
}
