using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Operations;

namespace MagicMedia.Store
{
    public interface IOperationStore
    {
        Task AddAsync(MediaOperation operation, CancellationToken cancellationToken);
        Task<MediaOperation> GetAsync(Guid operationId, CancellationToken cancellationToken);
        Task UpdateAsync(MediaOperation operation, CancellationToken cancellationToken);
        Task UpdateTaskAsync(Guid operationId, MediaOperationTask task, CancellationToken cancellationToken);
    }
}
