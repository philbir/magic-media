using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IMediaService
    {
        MediaThumbnail? GetThumbnail(Media media, ThumbnailSizeName size);
        Task<MediaThumbnail?> GetThumbnailAsync(Guid mediaId, ThumbnailSizeName size, CancellationToken cancellationToken);
        Task<Media> UpdateDateTakenAsync(Guid id, DateTimeOffset? dateTaken, CancellationToken cancellationToken);
    }
}