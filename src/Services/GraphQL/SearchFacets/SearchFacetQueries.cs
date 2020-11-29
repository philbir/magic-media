using System.Threading;
using HotChocolate;
using HotChocolate.Types;

namespace MagicMedia.GraphQL.SearchFacets
{
    [ExtendObjectType(Name = "Query")]
    public class SearchFacetQueries
    {
        static readonly object NoOp = new object();

        [GraphQLType(typeof(SearchFacetType))]
        public object GetFacets(CancellationToken cancellationToken)
        {
            return NoOp;
        }
    }
}
