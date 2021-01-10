using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia
{
    internal class DownloadMediaProfile
    {
        public static DownloadMediaOptions CreateOptions(string? name)
        {
            switch (name)
            {
                case "SOCIAL_MEDIA":
                    return new DownloadMediaOptions
                    {
                        ImageSize = ImageDownloadSize.Medium,
                        JpegCompression = 80,
                        RemoveMetadata = true
                    };
                default:
                    return new DownloadMediaOptions
                    {
                        ImageSize = ImageDownloadSize.Original,
                        VideoSize = VideoDownloadSize.Original,
                        RemoveMetadata = false
                    };
            }
        }
    }

    public static class DownloadImageSizeMapExtensions
    {
        public static int GetMaxValue(this ImageDownloadSize size)
        {
            switch (size)
            {
                case ImageDownloadSize.Medium:
                    return 1280;
                case ImageDownloadSize.Small:
                    return 800;
                default:
                    return int.MaxValue;
            }
        }
    }

    public class MediaDownloadService : IMediaDownloadService
    {
        private readonly IMediaService _mediaService;

        public MediaDownloadService(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<MediaDownload> CreateDownloadAsync(
            Guid id,
            string? profile,
            CancellationToken cancellationToken)
        {
            DownloadMediaOptions options = DownloadMediaProfile.CreateOptions(profile);

            return await CreateDownloadAsync(id, options, cancellationToken);
        }

        public async Task<MediaDownload> CreateDownloadAsync(
            Guid id,
            DownloadMediaOptions options,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaService.GetByIdAsync(id, cancellationToken);

            Stream stream = _mediaService.GetMediaStream(media);

            if (HasModifiers(options))
            {
                stream = await ProcessMediaAsync(stream, media, options, cancellationToken);
            }

            return new MediaDownload(stream, CreateFilename(media));
        }

        private async Task<Stream> ProcessMediaAsync(
            Stream stream,
            Media media,
            DownloadMediaOptions options,
            CancellationToken cancellationToken)
        {
            if (media.MediaType == MediaType.Image)
            {
                return await ProcessImageAsync(stream, media, options, cancellationToken);
            }

            throw new NotImplementedException();
        }

        private async Task<Stream> ProcessImageAsync(
            Stream stream,
            Media media,
            DownloadMediaOptions options,
            CancellationToken cancellationToken)
        {
            Image image = await Image.LoadAsync(stream);

            if (options.ImageSize != ImageDownloadSize.Original)
            {
                Size? newSize = null;
                Size currentSize = image.Size();

                MediaOrientation orientation = image.GetOrientation();
                if (orientation == MediaOrientation.Landscape)
                {
                    var maxWidth = options.ImageSize.GetMaxValue();
                    if (currentSize.Width > maxWidth)
                    {
                        newSize = new Size(
                            maxWidth,
                            currentSize.Width / maxWidth * currentSize.Height);
                    }
                }

                if ( newSize != null)
                {
                    image.Mutate(x => x.Resize(newSize.Value));
                }
            }

            if (options.RemoveMetadata)
            {
                image.Metadata.ExifProfile = null;
            }

            var ms = new MemoryStream();
            if (options.JpegCompression.HasValue)
            {
                var encoder = new JpegEncoder()
                {
                    Quality = options.JpegCompression.Value
                };
                await image.SaveAsJpegAsync(ms, encoder, cancellationToken);
            }
            else
            {
                await image.SaveAsJpegAsync(ms, cancellationToken);
            }

            return ms;
        }

        private string CreateFilename(Media media)
        {
            StringBuilder sb = new StringBuilder();
            if (media.Folder != null)
            {
                var folders = media.Folder.Split('/', StringSplitOptions.RemoveEmptyEntries);

                sb.Append(string.Join("_", folders));
                sb.Append("_");
            }

            sb.Append(media.Filename);

            return RemoveIllegalChars(sb.ToString());
        }

        private string RemoveIllegalChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }

        public bool HasModifiers(DownloadMediaOptions options)
        {
            return
                options.ImageSize != ImageDownloadSize.Original ||
                options.JpegCompression.HasValue ||
                options.RemoveMetadata ||
                options.VideoSize != VideoDownloadSize.Original;
        }
    }
}
