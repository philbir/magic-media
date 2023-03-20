using MagicMedia.Search;

namespace MagicMedia.GraphQL.SearchFacets;

public class SearchFacetType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor
            .Field("country")
            .ResolveWith<Resolvers>(x => x.GetCountriesAsync(default!, default!));

        descriptor
            .Field("city")
            .ResolveWith<Resolvers>(x => x.GetCitiesAsync(default!, default!));

        descriptor
            .Field("camera")
            .ResolveWith<Resolvers>(x => x.GetCamerasAsync(default!, default!));

        descriptor
           .Field("aiTags")
           .ResolveWith<Resolvers>(x => x.GetAITagsAsync(default!, default!));

        descriptor
           .Field("aiObjects")
           .ResolveWith<Resolvers>(x => x.GetAIObjectsAsync(default!, default!));

        descriptor
            .Field("tags")
            .ResolveWith<Resolvers>(x => x.GetTagDefinitionsAsync(default!, default!));
    }

    private class Resolvers
    {
        public Task<IEnumerable<SearchFacetItem>> GetCountriesAsync(
            [Service] ISearchFacetService searchFacetService,
            CancellationToken cancellationToken)
        {
            return searchFacetService.GetCountryFacetsAsync(cancellationToken);
        }

        public Task<IEnumerable<SearchFacetItem>> GetCitiesAsync(
            [Service] ISearchFacetService searchFacetService,
            CancellationToken cancellationToken)
        {
            return searchFacetService.GetCityFacetsAsync(cancellationToken);
        }

        public Task<IEnumerable<SearchFacetItem>> GetCamerasAsync(
            [Service] ISearchFacetService searchFacetService,
            CancellationToken cancellationToken)
        {
            return searchFacetService.GetCameraFacetsAsync(cancellationToken);
        }

        public Task<IEnumerable<SearchFacetItem>> GetAITagsAsync(
            [Service] ISearchFacetService searchFacetService,
            CancellationToken cancellationToken)
        {
            return searchFacetService.GetAITagFacetsAsync(cancellationToken);
        }

        public Task<IEnumerable<SearchFacetItem>> GetAIObjectsAsync(
            [Service] ISearchFacetService searchFacetService,
            CancellationToken cancellationToken)
        {
            return searchFacetService.GetAIObjectsFacetsAsync(cancellationToken);
        }

        public Task<IEnumerable<SearchFacetItem>> GetTagDefinitionsAsync(
            [Service] ISearchFacetService searchFacetService,
            CancellationToken cancellationToken)
        {
            return searchFacetService.GetTagDefinitionsAsync(cancellationToken);
        }
    }
}
