using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Operations
{
    public interface IMediaOperationTaskHandler
    {
        Task ExecuteAsync(
            Guid operationid,
            MediaOperationTask task,
            CancellationToken cancellationToken);
    }
}
