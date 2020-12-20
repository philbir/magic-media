using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.GraphQL.SearchFacets
{
    public class SearchFacetResolvers
    {
        private readonly ISearchFacetService _searchFacetService;

        public SearchFacetResolvers(ISearchFacetService searchFacetService)
        {
            _searchFacetService = searchFacetService;
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCountriesAsync(
            CancellationToken cancellationToken)
        {
            return await _searchFacetService.GetCountryFacetsAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCitiesAsync(
            CancellationToken cancellationToken)
        {
            return await _searchFacetService.GetCityFacetsAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCamerasAsync(
            CancellationToken cancellationToken)
        {
            return await _searchFacetService.GetCameraFacetsAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAITagsAsync(
            CancellationToken cancellationToken)
        {
            return await _searchFacetService.GetAITagFacetsAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAIObjectsAsync(
            CancellationToken cancellationToken)
        {
            return await _searchFacetService.GetAIObjectsFacetsAsync(cancellationToken);
        }
    }
}
