using MagicMedia.Search;

namespace MagicMedia.GraphQL.SearchFacets
{
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
        }

        public class Resolvers
        {


            public async Task<IEnumerable<SearchFacetItem>> GetCountriesAsync(
                [Service] ISearchFacetService searchFacetService,
                CancellationToken cancellationToken)
            {
                return await searchFacetService.GetCountryFacetsAsync(cancellationToken);
            }

            public async Task<IEnumerable<SearchFacetItem>> GetCitiesAsync(
                [Service] ISearchFacetService searchFacetService,
                CancellationToken cancellationToken)
            {
                return await searchFacetService.GetCityFacetsAsync(cancellationToken);
            }

            public async Task<IEnumerable<SearchFacetItem>> GetCamerasAsync(
                [Service] ISearchFacetService searchFacetService,
                CancellationToken cancellationToken)
            {
                return await searchFacetService.GetCameraFacetsAsync(cancellationToken);
            }

            public async Task<IEnumerable<SearchFacetItem>> GetAITagsAsync(
                [Service] ISearchFacetService searchFacetService,
                CancellationToken cancellationToken)
            {
                return await searchFacetService.GetAITagFacetsAsync(cancellationToken);
            }

            public async Task<IEnumerable<SearchFacetItem>> GetAIObjectsAsync(
                [Service] ISearchFacetService searchFacetService,
                CancellationToken cancellationToken)
            {
                return await searchFacetService.GetAIObjectsFacetsAsync(cancellationToken);
            }
        }
    }
}
