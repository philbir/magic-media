using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface IMediaSearchService
    {
        Task<IEnumerable<GeoClusterLocation>> GetGeoLocationClustersAsync(GetGeoLocationClustersRequest request, CancellationToken cancellationToken);
        Task<SearchResult<Media>> SearchAsync(SearchMediaRequest request, CancellationToken cancellationToken);
    }
}
