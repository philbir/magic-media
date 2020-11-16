using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IAlbumSummaryService
    {
        Task<Album> BuildAsync(Album album, CancellationToken cancellationToken);
        Task<Album> UpdateAsync(Guid id, CancellationToken cancellationToken);
    }
}