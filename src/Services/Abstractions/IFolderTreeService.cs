using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia;

public interface IFolderTreeService
{
    Task<FolderItem> GetTreeAsync(CancellationToken cancellationToken);
}
