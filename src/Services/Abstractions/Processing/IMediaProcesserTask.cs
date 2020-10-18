using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing
{
    public interface IMediaProcesserTask
    {
        public string Name { get; }

        public Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken);
    }
}
