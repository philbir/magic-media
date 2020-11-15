using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store.MongoDb
{
    public interface IAlbumStore
    {
        Task<Album> AddAsync(Album album, CancellationToken cancellationToken);
        Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken);
        Task<Album> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<SearchResult<Album>> SearchAsync(SearchAlbumRequest request, CancellationToken cancellationToken);
        Task<Album> UpdateAsync(Album album, CancellationToken cancellationToken);
    }
}