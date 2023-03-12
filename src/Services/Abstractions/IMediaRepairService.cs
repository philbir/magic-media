using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IMediaRepairService
{
    Task ApplyRepairsAsync(Media media, ConsistencyCheck check, CancellationToken cancellationToken);
}
