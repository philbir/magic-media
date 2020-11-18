using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IMediaBlobStore
    {
        public Task StoreAsync(MediaBlobData data, CancellationToken cancellationToken);

        public Task<MediaBlobData> GetAsync(
            MediaBlobData request,
            CancellationToken cancellationToken);

        Task MoveAsync(
            MediaBlobData request,
            string newLocation,
            CancellationToken cancellationToken);

        Task MoveToSpecialFolderAsync(
            MediaBlobData request,
            MediaBlobType mediaBlobType,
            CancellationToken cancellationToken);
    }
}
