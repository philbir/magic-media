using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace MagicMedia;

public interface IBoxExtractorService
{
    Task<IEnumerable<BoxExtractionResult>> ExtractBoxesAsync(
        Image image,
        IEnumerable<BoxExtractionInput> inputs,
        ThumbnailSizeName thumbnailSize,
        CancellationToken cancellationToken);

    Task<IEnumerable<BoxExtractionResult>> ExtractBoxesAsync(
        Stream stream,
        IEnumerable<BoxExtractionInput> inputs,
        ThumbnailSizeName thumbnailSize,
        CancellationToken cancellationToken);
}
