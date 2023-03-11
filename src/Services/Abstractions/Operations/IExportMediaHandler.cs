using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;

namespace MagicMedia.Operations;

public interface IExportMediaHandler
{
    Task ExecuteAsync(ExportMediaMessage message, CancellationToken cancellationToken);
}
