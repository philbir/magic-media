using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store;

public interface IMediaAIStore
{
    Task<MediaAI> GetByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken);

    Task<IEnumerable<SearchFacetItem>> GetGroupedAIObjectsAsync(
        IEnumerable<Guid>? mediaIds,
        CancellationToken cancellationToken);

    Task<IEnumerable<SearchFacetItem>> GetGroupedAITagsAsync(
        IEnumerable<Guid>? mediaIds,
        CancellationToken cancellationToken);

    Task<IEnumerable<MediaAI>> GetWithoutSourceInfoAsync(
        AISource source,
        int limit,
        bool excludePersons,
        CancellationToken cancellationToken);

    Task<MediaAI> SaveAsync(MediaAI mediaAI, CancellationToken cancellationToken);
}
