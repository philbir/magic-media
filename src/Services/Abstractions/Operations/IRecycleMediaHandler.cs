using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;

namespace MagicMedia.Operations
{
    public interface IRecycleMediaHandler
    {
        Task ExecuteAsync(RecycleMediaMessage message, CancellationToken cancellationToken);
    }
}
