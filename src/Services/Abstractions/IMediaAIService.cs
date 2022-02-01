using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.ImageAI;
using MagicMedia.Store;

namespace MagicMedia;

public interface IMediaAIService
{
    Task<MediaAI?> AnalyseMediaAsync(Guid mediaId, CancellationToken cancellationToken);
    Task<MediaAI?> AnalyseMediaAsync(Media media, CancellationToken cancellationToken);
    Task<IEnumerable<Media>> GetMediaIdsForImageAIJobAsync(int limit, CancellationToken cancellationToken);
    Task ProcessNewBySourceAsync(AISource source, CancellationToken cancellationToken);
    Task SaveImageAIDetectionAsync(ImageAIDetectionResult imageAIResult, CancellationToken cancellationToken);
}
