using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IMediaDownloadService
    {
        Task<MediaDownload> CreateDownloadAsync(
            Guid id,
            DownloadMediaOptions options,
            CancellationToken cancellationToken);
        Task<MediaDownload> CreateDownloadAsync(Guid id, string profile, CancellationToken cancellationToken);
    }
}
