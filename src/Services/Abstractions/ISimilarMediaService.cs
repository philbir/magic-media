using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface ISimilarMediaService
    {
        Task GetDuplicatesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SimilarMediaGroup>> GetSimilarMediaGroupsAsync(SearchSimilarMediaRequest request, CancellationToken cancellationToken);
    }
}
