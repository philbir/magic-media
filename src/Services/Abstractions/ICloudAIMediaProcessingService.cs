using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface ICloudAIMediaProcessingService
    {
        Task<MediaAI?> AnalyseMediaAsync(Guid mediaId, CancellationToken cancellationToken);
        Task<MediaAI?> AnalyseMediaAsync(Media media, CancellationToken cancellationToken);
        Task ProcessNewBySourceAsync(AISource source, CancellationToken cancellationToken);
    }
}
