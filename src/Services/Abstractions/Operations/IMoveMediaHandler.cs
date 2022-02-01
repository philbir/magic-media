using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;

namespace MagicMedia.Operations;

public interface IMoveMediaHandler
{
    Guid MediaId { get; }

    Task ExecuteAsync(MoveMediaMessage message, CancellationToken cancellationToken);
}
