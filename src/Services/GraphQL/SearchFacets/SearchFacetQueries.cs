using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;

namespace MagicMedia.GraphQL.SearchFacets
{
    [ExtendObjectType(Name = "Query")]
    public class SearchFacetQueries
    {
        public async Task<SearchFacets> GetFacets(CancellationToken cancellationToken)
        {
            return new SearchFacets();
        }
    }
}
