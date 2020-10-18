using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing
{
    public interface IMediaProcessorFlow
    {
        Task ExecuteAsync(MediaProcessorContext context, CancellationToken cancellationToken);
    }
}