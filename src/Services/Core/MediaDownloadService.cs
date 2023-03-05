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

namespace MagicMedia;

public class MediaDownloadService : IMediaDownloadService
{
    private readonly IMediaService _mediaService;
    private readonly IMediaStore _mediaStore;

    public MediaDownloadService(
        IMediaService mediaService,
        IMediaStore mediaStore)
    {
        _mediaService = mediaService;
        _mediaStore = mediaStore;
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

        Stream? resultStream;

        if (media.MediaType == MediaType.Image)
        {
            Stream mediaStream = _mediaService.GetMediaStream(media);

            if (HasModifiers(options))
            {
                resultStream = await ProcessImageAsync(
                    mediaStream,
                    media,
                    options,
                    cancellationToken);

                mediaStream.Close();
            }
            else
            {
                resultStream = mediaStream;
            }
        }
        else
        {
            if (options.VideoSize == VideoDownloadSize.Video720)
            {
                MediaBlobData request = _mediaService.GetBlobRequest(
                    media,
                    MediaFileType.Video720);

                resultStream = _mediaStore.Blob.GetStreamAsync(request);
            }
            else
            {
                resultStream = _mediaService.GetMediaStream(media);
            }
        }

        return new MediaDownload(resultStream, CreateFilename(media));
    }

    private async Task<Stream> ProcessImageAsync(
        Stream stream,
        Media media,
        DownloadMediaOptions options,
        CancellationToken cancellationToken)
    {
        Image image = await Image.LoadAsync(stream, cancellationToken);

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
                        (int)(maxWidth / (double)currentSize.Width * currentSize.Height));
                }
            }
            else
            {
                var maxHeight = options.ImageSize.GetMaxValue();
                if (currentSize.Height > maxHeight)
                {
                    newSize = new Size(
                        (int)(maxHeight / (double)currentSize.Height * currentSize.Width),
                        maxHeight);
                }
            }

            if (newSize != null)
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
        ms.Position = 0;

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
