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
using Serilog;
using SixLabors.ImageSharp;

namespace MagicMedia
{
    public class MediaService : IMediaService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IAgeOperationsService _ageOperationsService;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;
        private readonly IBus _bus;

        public MediaService(
            IMediaStore mediaStore,
            IMediaBlobStore mediaBlobStore,
            IAgeOperationsService ageOperationsService,
            IThumbnailBlobStore thumbnailBlobStore,
            IBus bus)
        {
            _mediaStore = mediaStore;
            _mediaBlobStore = mediaBlobStore;
            _ageOperationsService = ageOperationsService;
            _thumbnailBlobStore = thumbnailBlobStore;
            _bus = bus;
        }

        public async Task<Media> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<MediaBlobData> GetMediaData(Media media, CancellationToken cancellationToken)
        {
            MediaBlobData blob = await _mediaBlobStore.GetAsync(
                media.ToBlobDataRequest(),
                cancellationToken);

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

            if (request.WebImage != null)
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


            if (request.SaveMode == SaveMediaMode.CreateNew)
            {
                if (request.Media.MediaType == MediaType.Image)
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
                else
                {
                    var newFileName = GetFilename(request.Media, MediaFileType.Original);

                    DirectoryInfo directory = new FileInfo(newFileName).Directory!;
                    if (directory!.Exists)
                    {
                        directory.Create();
                    }

                    File.Copy(request.Media.Source.Identifier, newFileName);
                }
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

        public async Task DeleteAsync(Media media, CancellationToken cancellationToken)
        {
            Log.Information("Delete media {Id}", media.Id);

            await DeleteAllFilesAsync(media, cancellationToken);

            await DeleteThumbnailsAsync(media, cancellationToken);

            await _mediaStore.DeleteAsync(media.Id, cancellationToken);

            await _bus.Publish(new MediaDeletedMessage(media.Id));
        }

        private async Task DeleteThumbnailsAsync(Media media, CancellationToken cancellationToken)
        {
            foreach (MediaThumbnail thumb in media.Thumbnails)
            {
                await _thumbnailBlobStore.DeleteAsync(thumb.Id, cancellationToken);
            }
        }

        private async Task DeleteAllFilesAsync(Media media, CancellationToken cancellationToken)
        {
            await _mediaBlobStore.DeleteAsync(
                GetBlobRequest(media, MediaFileType.Original),
                cancellationToken);

            await _mediaBlobStore.DeleteAsync(
                GetBlobRequest(media, MediaFileType.WebPreview),
                cancellationToken);

            if (media.MediaType == MediaType.Video)
            {
                await _mediaBlobStore.DeleteAsync(
                    GetBlobRequest(media, MediaFileType.VideoGif),
                    cancellationToken);

                await _mediaBlobStore.DeleteAsync(
                    GetBlobRequest(media, MediaFileType.Video720),
                    cancellationToken);
            }
        }

        public async Task<Media> UpdateDateTakenAsync(
            Guid id,
            DateTimeOffset? dateTaken,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);
            bool hasChanged = media.DateTaken != dateTaken;
            media.DateTaken = dateTaken;
            media.DateTakenSearch = DateSearch.Create(dateTaken);

            await _mediaStore.UpdateAsync(media, cancellationToken);

            if (hasChanged)
            {
                await _ageOperationsService.UpdateAgesByMediaAsync(media, cancellationToken);
            }

            return media;
        }

        public MediaBlobData GetBlobRequest(Media media, MediaFileType type)
        {
            switch (type)
            {
                case MediaFileType.Original:
                    return media.ToBlobDataRequest();
                case MediaFileType.WebPreview:
                    return new MediaBlobData
                    {
                        Type = MediaBlobType.Web,
                        Filename = $"{media.Id.ToString("N")}.webp"
                    };
                case MediaFileType.VideoGif:
                    return new MediaBlobData
                    {
                        Type = MediaBlobType.VideoPreview,
                        Filename = $"{media.Id}.gif"
                    };
                case MediaFileType.Video720:
                    return new MediaBlobData
                    {
                        Type = MediaBlobType.VideoPreview,
                        Filename = $"720P_{media.Id}.mp4"
                    };
                default:
                    throw new ArgumentException($"Unknown type: {type}");
            }
        }

        public string GetFilename(Media media, MediaFileType mediaFileType)
        {
            return _mediaBlobStore.GetFilename(GetBlobRequest(media, mediaFileType));
        }

        public IEnumerable<MediaFileInfo> GetMediaFiles(
            Media media)
        {
            var infos = new List<MediaFileInfo>();

            foreach (MediaFileType type in (MediaFileType[])Enum.GetValues(typeof(MediaFileType)))
            {
                MediaBlobData? request = GetBlobRequest(media, type);
                var fileInfo = new FileInfo(_mediaBlobStore.GetFilename(request));

                if (fileInfo.Exists)
                {
                    infos.Add(new MediaFileInfo(
                        type,
                        fileInfo.DirectoryName!,
                        fileInfo.Name,
                        fileInfo.Length));
                }
            }

            return infos;
        }

        public Task<MediaAI> GetAIDataAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            return _mediaStore.MediaAI.GetByMediaIdAsync(mediaId, cancellationToken);
        }
    }
}
