using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;

namespace MagicMedia.Operations
{
    public interface IDeleteMediaHandler
    {
        Task ExecuteAsync(DeleteMediaMessage message, CancellationToken cancellationToken);
    }
}
