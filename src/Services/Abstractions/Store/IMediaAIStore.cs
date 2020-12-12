using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store
{
    public interface IMediaAIStore
    {
        Task<MediaAI> GetByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken);

        Task<IEnumerable<SearchFacetItem>> GetGroupedAIObjectsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SearchFacetItem>> GetGroupedAITagsAsync(CancellationToken cancellationToken);
    }
}
