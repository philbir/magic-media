using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Discovery
{
    public interface IMediaSourceDiscovery
    {
        MediaDiscoverySource SourceType { get; }

        Task DeleteMediaAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<MediaDiscoveryIdentifier>> DiscoverMediaAsync(CancellationToken cancellationToken);
        Task<byte[]> GetMediaDataAsync(string id, CancellationToken cancellationToken);
        Task SaveMediaDataAsync(string id, byte[] data, CancellationToken cancellationToken);
    }
}
