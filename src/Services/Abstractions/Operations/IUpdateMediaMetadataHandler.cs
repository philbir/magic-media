using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;

namespace MagicMedia.Operations;

public interface IUpdateMediaMetadataHandler
{
    Task ExecuteAsync(UpdateMediaMetadataMessage message, CancellationToken cancellationToken);
}
