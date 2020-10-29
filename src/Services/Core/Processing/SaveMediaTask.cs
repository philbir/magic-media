using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing
{
    public class SaveMediaTask : IMediaProcesserTask
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _blobStore;
        private readonly ICameraService _cameraService;

        public SaveMediaTask(
            IMediaStore mediaStore,
            IMediaBlobStore blobStore,
            ICameraService cameraService)
        {
            _mediaStore = mediaStore;
            _blobStore = blobStore;
            _cameraService = cameraService;
        }

        public string Name => MediaProcessorTaskNames.SaveMedia;

        private string GetFolder(MediaProcessorContext context)
        {
            if (context.Metadata.DateTaken.HasValue)
            {
                return string.Join("/",
                    "New",
                    context.Metadata.DateTaken.Value.Year.ToString(),
                    context.Metadata.DateTaken.Value.ToString("MMMM", new CultureInfo("en-US")));
            }
            else
            {
                return "New/Unknown_Date";
            }
        }

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {

            var media = new Media
            {
                Id = Guid.NewGuid(),
                MediaType = MediaType.Image,
                DateTaken = context.Metadata.DateTaken,
                ImageUniqueId = context.Metadata.ImageId,
                GeoLocation = context.Metadata.GeoLocation,
                Size = context.OriginalData.Length,
                Folder = GetFolder(context),
                Filename = Path.GetFileName(context.File.Id),
                Dimension = context.Metadata.Dimension,
                OriginalHash = ComputeHash(context.OriginalData),
                CameraId = await GetCameraIdAsync(context, cancellationToken),
                Source = new MediaSource
                {
                    Identifier = context.File.Id,
                    Type = context.File.Source.ToString(),
                    ImportedAt = DateTime.UtcNow
                }
            };

            if (context.Thumbnails is { })
            {
                media.Thumbnails = context.Thumbnails.Select(x => new MediaThumbnail
                {
                    Id = Guid.NewGuid(),
                    Data = x.Data,
                    Format = x.Format,
                    Dimensions = x.Dimensions,
                    Size = x.Size
                }).ToList();
            }
            IEnumerable<MediaFace> faces = new List<MediaFace>();

            if (context.FaceData is { })
            {
                faces = context.FaceData.Select(f => new MediaFace
                {
                    Box = f.Box,
                    Id = f.Id,
                    Encoding = f.Encoding,
                    MediaId = media.Id,
                    RecognitionType = f.RecognitionType,
                    State = FaceState.New,
                    Thumbnail = f.Thumbnail,
                    PersonId = f.PersonId,
                    DistanceThreshold = f.DistanceThreshold
                }).ToList();

                media.FaceCount = faces.Count();
            }

            await _mediaStore.InsertMediaAsync(media, faces, default);

            await _blobStore.StoreAsync(
                new MediaBlobData
                {
                    Type = MediaBlobType.Web,
                    Data = context.WebImage,
                    Filename = $"{media.Id.ToString("N")}.webp",
                },
                cancellationToken);

            MemoryStream stream = new MemoryStream();
            await context.Image.SaveAsJpegAsync(stream, cancellationToken);
            stream.Position = 0;

            await _blobStore.StoreAsync(
                new MediaBlobData
                {
                    Type = MediaBlobType.Media,
                    Data = stream.ToArray(),
                    Directory = media.Folder,
                    Filename = Path.GetFileName(context.File.Id)
                },
                cancellationToken);
        }

        private async Task<Guid?> GetCameraIdAsync(MediaProcessorContext context, CancellationToken cancellationToken)
        {
            if (context.Metadata.Camera is { })
            {
                Camera? camera = await _cameraService.GetOrCreateAsync(
                    context.Metadata.Camera.Make,
                    context.Metadata.Camera.Model,
                    cancellationToken);

                return camera.Id;
            }

            return null;
        }

        public string ComputeHash(byte[] data)
        {
            var sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(data);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }
    }
}
