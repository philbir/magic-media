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
    }
}
