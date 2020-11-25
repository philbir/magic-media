using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IMediaService
    {
        Task AddNewMediaAsync(MagicMedia.AddNewMediaRequest request, CancellationToken cancellationToken);
        Task<Media> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<MediaBlobData> GetMediaData(Media media, CancellationToken cancellationToken);
        Stream GetMediaStream(Media media);
        MediaThumbnail? GetThumbnail(Media media, ThumbnailSizeName size);
        Task<MediaThumbnail?> GetThumbnailAsync(Guid mediaId, ThumbnailSizeName size, CancellationToken cancellationToken);
        Task<Media> UpdateDateTakenAsync(Guid id, DateTimeOffset? dateTaken, CancellationToken cancellationToken);
    }
}