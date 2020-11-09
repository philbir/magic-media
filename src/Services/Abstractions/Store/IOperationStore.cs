using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Operations;

namespace MagicMedia.Store
{
    public interface IOperationStore
    {
        Task AddAsync(MediaOperation operation, CancellationToken cancellationToken);
    }
}
