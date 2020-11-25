using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Processing;
using MagicMedia.Store;
using MassTransit;
using SixLabors.ImageSharp;

namespace MagicMedia
{
    public class MediaService : IMediaService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IAgeOperationsService _ageOperationsService;
        private readonly IBus _bus;

        public MediaService(
            IMediaStore mediaStore,
            IMediaBlobStore mediaBlobStore,
            IAgeOperationsService ageOperationsService,
            IBus bus)
        {
            _mediaStore = mediaStore;
            _mediaBlobStore = mediaBlobStore;
            _ageOperationsService = ageOperationsService;
            _bus = bus;
        }

        public async Task<Media> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<MediaBlobData> GetMediaData(Media media, CancellationToken cancellationToken)
        {
            MediaBlobData blob = await _mediaBlobStore.GetAsync(media.ToBlobDataRequest(), cancellationToken);

            return blob;
        }

        public Stream GetMediaStream(Media media)
        {
            Stream stream = _mediaBlobStore.GetStreamAsync(media.ToBlobDataRequest());

            return stream;
        }

        public async Task AddNewMediaAsync(AddNewMediaRequest request, CancellationToken cancellationToken)
        {
            await _mediaStore.InsertMediaAsync(
                request.Media,
                request.Faces,
                cancellationToken);

            if ( request.WebImage != null)
            {
                await _mediaBlobStore.StoreAsync(
                  new MediaBlobData
                  {
                      Type = MediaBlobType.Web,
                      Data = request.WebImage,
                      Filename = $"{request.Media.Id.ToString("N")}.webp",
                  },
                  cancellationToken);
            }

            if (request.Image != null && request.SaveMode == SaveMediaMode.CreateNew)
            {
                MemoryStream stream = new MemoryStream();
                await request.Image.SaveAsJpegAsync(stream, cancellationToken);
                stream.Position = 0;

                await _mediaBlobStore.StoreAsync(
                    new MediaBlobData
                    {
                        Type = MediaBlobType.Media,
                        Data = stream.ToArray(),
                        Directory = request.Media.Folder,
                        Filename = Path.GetFileName(request.Media.Filename)
                    },
                    cancellationToken);
            }

            await _bus.Publish(new NewMediaAddedMessage(request.Media.Id));
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
