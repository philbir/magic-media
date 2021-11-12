namespace MagicMedia.GraphQL.SearchFacets
{
    [ExtendObjectType(RootTypes.Query)]
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
