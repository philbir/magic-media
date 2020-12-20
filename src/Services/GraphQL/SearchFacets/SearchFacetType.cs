using HotChocolate.Types;

namespace MagicMedia.GraphQL.SearchFacets
{
    public class SearchFacetType : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor
                .Field("country")
                .ResolveWith<SearchFacetResolvers>(x => x.GetCountriesAsync(default!));

            descriptor
                .Field("city")
                .ResolveWith<SearchFacetResolvers>(x => x.GetCitiesAsync(default!));

            descriptor
                .Field("camera")
                .ResolveWith<SearchFacetResolvers>(x => x.GetCamerasAsync(default!));

            descriptor
               .Field("aiTags")
               .ResolveWith<SearchFacetResolvers>(x => x.GetAITagsAsync(default!));

            descriptor
               .Field("aiObjects")
               .ResolveWith<SearchFacetResolvers>(x => x.GetAIObjectsAsync(default!));
        }
    }
}
