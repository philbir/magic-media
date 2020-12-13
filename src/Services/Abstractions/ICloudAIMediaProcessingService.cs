using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface ICloudAIMediaProcessingService
    {
        Task AnalyseMediaAsync(Media media, CancellationToken cancellationToken);
        Task ProcessNewBySourceAsync(AISource source, CancellationToken cancellationToken);
    }
}