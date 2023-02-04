using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace MagicMedia.Thumbnail;

public interface IThumbnailService
{
    Task<IEnumerable<ThumbnailResult>> GenerateAllThumbnailAsync(
        Image image,
        CancellationToken cancellationToken);
    Task<IEnumerable<ThumbnailResult>> GenerateAllThumbnailAsync(Stream stream, CancellationToken cancellationToken);
    Task<ThumbnailResult> GenerateThumbnailAsync(
        Image image,
        ThumbnailSizeName size,
        CancellationToken cancellationToken);

    Task<ThumbnailResult> GenerateThumbnailAsync(
        Stream stream,
        ThumbnailSizeName size,
        CancellationToken cancellationToken);
}
