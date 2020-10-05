using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IMetadataExtractor
    {
        Task<MediaMetadata> GetMetadataAsync(Stream stream, CancellationToken cancellationToken);
    }
}