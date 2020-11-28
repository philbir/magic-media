using System.IO;
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
        Stream GetStreamAsync(MediaBlobData request);
        string GetFilename(MediaBlobData data);
        Task<bool> DeleteAsync(MediaBlobData request, CancellationToken cancellationToken);
    }
}
