using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IMediaRepairService
{
    Task GetPossibleRepairsAsync(Media media, ConsistencyCheck check, CancellationToken cancellationToken);
    Task<Media> ExecuteRepairAsync(RepairMediaRequest request, CancellationToken cancellationToken);
}

