using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia
{
    public class SearchFacetService : ISearchFacetService
    {
        private readonly IMediaStore _mediaStore;

        public SearchFacetService(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCountryFacetsAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStore.GetGroupedCountriesAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCityFacetsAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStore.GetGroupedCitiesAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAITagFacetsAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStore.MediaAI.GetGroupedAITagsAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAIObjectsFacetsAsync(
            CancellationToken cancellationToken)
        {
            return await _mediaStore.MediaAI.GetGroupedAIObjectsAsync(cancellationToken);
        }
    }
}
