using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;

namespace MagicMedia.Processing;

public interface IMediaSourceScanner
{
    Task ProcessFileAsync(MediaDiscoveryIdentifier file, CancellationToken cancellationToken);
    Task ScanAsync(CancellationToken cancellationToken);
}
