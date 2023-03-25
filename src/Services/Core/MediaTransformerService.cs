using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class MediaTransformService : IMediaTransformService
{
    private readonly IMediaService _mediaService;

    public MediaTransformService(
        IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<TransformedMedia> TransformAsync(
        Guid id,
        MediaTransform? transform,
        CancellationToken cancellationToken)
    {
        Media media = await _mediaService.GetByIdAsync(id, cancellationToken);

        transform ??= new MediaTransform { Format = "JPG" };

        if (media.MediaType == MediaType.Image)
        {
            await using Stream mediaStream = _mediaService.GetMediaStream(media);
            using Image image = await Image.LoadAsync(mediaStream, cancellationToken);

            if (transform.Resize is { })
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(transform.Resize.Size.Width, transform.Resize.Size.Height),
                    Mode = Enum.Parse<ResizeMode>(transform.Resize.Mode, ignoreCase: true)
                }));
            }

            if (transform.RemoveMetadata)
            {
                image.Metadata.ExifProfile = null;
            }

            IImageEncoder encoder = null;

            switch (transform.Format)
            {
                case "PNG":
                    encoder = new PngEncoder();
                    break;
                case "GIF":
                    encoder = new GifEncoder();
                    break;
                case "WEBP":
                    encoder = new WebpEncoder() { Quality = transform.Quality > 0 ? transform.Quality : 100 };
                    break;
                default:
                    encoder = new JpegEncoder { Quality = transform.Quality > 0 ? transform.Quality : 100 };
                    break;
            }

            using var transformedStream = new MemoryStream();
            await image.SaveAsync(transformedStream, encoder, cancellationToken);

            var transformedMedia = new TransformedMedia
            {
                Media = media,
                Data = transformedStream.ToArray(),
                Size = new MediaSize { Height = image.Height, Width = image.Width },
                Format = transform.Format ?? "JPG"
            };

            return transformedMedia;
        }

        // Video not yet supported
        return null;
    }
}
