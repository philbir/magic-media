using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store;

public interface ISimilarMediaStore
{
    Task AddAsync(IEnumerable<SimilarMediaInfo> similarInfos, CancellationToken cancellationToken);
    Task<IEnumerable<SimilarMediaGroup>> GetSimilarGroupsAsync(SearchSimilarMediaRequest request, CancellationToken cancellationToken);
}

public class SearchSimilarMediaRequest
{
    public MediaHashType HashType { get; set; }

    public double Similarity { get; set; }

    public int PageSize { get; set; }

    public int PageNr { get; set; }
}
