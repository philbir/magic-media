using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace MagicMedia;

public interface IMetadataExtractor
{
    Task<MediaMetadata> GetMetadataAsync(Stream stream, CancellationToken cancellationToken);
    Task<MediaMetadata> GetMetadataAsync(Image image, CancellationToken cancellationToken);
}
