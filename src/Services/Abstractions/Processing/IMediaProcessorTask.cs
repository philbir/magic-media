using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing;

public interface IMediaProcessorTask
{
    public string Name { get; }

    public Task ExecuteAsync(
        MediaProcessorContext context,
        CancellationToken cancellationToken);
}
