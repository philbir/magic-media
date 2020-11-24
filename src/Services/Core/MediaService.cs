using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public class MediaService : IMediaService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IAgeOperationsService _ageOperationsService;

        public MediaService(
            IMediaStore mediaStore,
            IAgeOperationsService ageOperationsService)
        {
            _mediaStore = mediaStore;
            _ageOperationsService = ageOperationsService;
        }

        public async Task<MediaThumbnail?> GetThumbnailAsync(Guid mediaId, ThumbnailSizeName size, CancellationToken cancellationToken)
        {
            Media media = await _mediaStore.GetByIdAsync(mediaId, cancellationToken);

            MediaThumbnail? thumb = GetThumbnail(media, size);

            if (thumb != null)
            {
                thumb.Data = await _mediaStore.Thumbnails.GetAsync(thumb.Id, cancellationToken);
            }

            return thumb;
        }

        public MediaThumbnail? GetThumbnail(Media media, ThumbnailSizeName size)
        {
            IEnumerable<MediaThumbnail>? thumbs = media.Thumbnails
                .Where(x => x.Size == size);

            MediaThumbnail? thumb = thumbs.Where(x => x.Format == "webp").FirstOrDefault() ??
                thumbs.FirstOrDefault();

            return thumb;
        }

        public async Task<Media> UpdateDateTakenAsync(
            Guid id,
            DateTimeOffset? dateTaken,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);
            bool hasChanged = media.DateTaken != dateTaken;
            media.DateTaken = dateTaken;

            await _mediaStore.UpdateAsync(media, cancellationToken);

            if (hasChanged)
            {
                await _ageOperationsService.UpdateAgesByMediaAsync(media, cancellationToken);
            }

            return media;
        }
    }
}
