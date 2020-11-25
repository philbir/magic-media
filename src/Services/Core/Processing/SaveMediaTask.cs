using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Processing
{
    public class SaveMediaTask : IMediaProcesserTask
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _blobStore;
        private readonly ICameraService _cameraService;
        private readonly IMediaService _mediaService;

        public SaveMediaTask(
            IMediaStore mediaStore,
            IMediaBlobStore blobStore,
            ICameraService cameraService,
            IMediaService mediaService)
        {
            _mediaStore = mediaStore;
            _blobStore = blobStore;
            _cameraService = cameraService;
            _mediaService = mediaService;
        }

        public string Name => MediaProcessorTaskNames.SaveMedia;

        private string GetFolder(MediaProcessorContext context)
        {
            switch (context.Options.SaveMedia.SaveMode)
            {
                case SaveMediaMode.CreateNew:
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
                case SaveMediaMode.KeepInSource:
                    var directoryName = Path.GetDirectoryName(context.File.Id);
                    var relativePath = directoryName.Replace(context.File.BasePath, "");

                    if (string.IsNullOrWhiteSpace(relativePath))
                    {
                        relativePath = "/";
                    }

                    return relativePath.Replace("\\", "/");
                default:
                    throw new ApplicationException($"Invalid SaveMediaMode: {context.Options.SaveMedia.SaveMode}");
            }
        }

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {
            var media = new Media
            {
                Id = Guid.NewGuid(),
                MediaType = context.MediaType,
                DateTaken = context.Metadata.DateTaken,
                ImageUniqueId = context.Metadata.ImageId,
                GeoLocation = context.Metadata.GeoLocation,
                Size = GetSize(context),
                Folder = GetFolder(context),
                Filename = Path.GetFileName(context.File.Id),
                Dimension = context.Metadata.Dimension,
                VideoInfo = context.VideoInfo,
                OriginalHash = ComputeHash(context),
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

            await _mediaService.AddNewMediaAsync(new AddNewMediaRequest(media)
            {
                Faces = faces.ToList(),
                Image = context.Image,
                SaveMode = context.Options.SaveMedia.SaveMode,
                WebImage = context.WebImage
            }, cancellationToken);

        }

        private long GetSize(MediaProcessorContext context)
        {
            if (context.Size > 0)
            {
                return context.Size;
            }
            else
            {
                return context.OriginalData!.Length;
            }
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

        private string ComputeHash(MediaProcessorContext context)
        {
            if (context.OriginalData != null)
            {
                return ComputeHash(context.OriginalData);
            }

            return ComputeHash(context.File.Id);
        }

        public string ComputeHash(byte[] data)
        {
            var sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(data);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }

        public string ComputeHash(string filename)
        {
            var sha = new SHA256Managed();
            using var stream = new FileStream(filename, FileMode.Open);

            byte[] checksum = sha.ComputeHash(stream);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }
    }
}
