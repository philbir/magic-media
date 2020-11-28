using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IMediaService
    {
        Task AddNewMediaAsync(MagicMedia.AddNewMediaRequest request, CancellationToken cancellationToken);
        MediaBlobData GetBlobRequest(Media media, MediaFileType type);
        Task<Media> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<MediaBlobData> GetMediaData(Media media, CancellationToken cancellationToken);
        IEnumerable<MediaFileInfo> GetMediaFiles(Media media);
        Stream GetMediaStream(Media media);
        MediaThumbnail? GetThumbnail(Media media, ThumbnailSizeName size);
        Task<MediaThumbnail?> GetThumbnailAsync(Guid mediaId, ThumbnailSizeName size, CancellationToken cancellationToken);
        Task<Media> UpdateDateTakenAsync(Guid id, DateTimeOffset? dateTaken, CancellationToken cancellationToken);
    }
}