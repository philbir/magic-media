using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store.MongoDb
{
    public interface IAlbumStore
    {
        Task<Album> AddAsync(Album album, CancellationToken cancellationToken);
        Task<Album> GetById(Guid id, CancellationToken cancellationToken);
        Task<Album> UpdateAsync(Album album, CancellationToken cancellationToken);
    }
}