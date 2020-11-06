using System;
using System.Linq;
using System.Text;
using HotChocolate.Types;

namespace MagicMedia.GraphQL.SearchFacets
{
    public class SearchFacetType : ObjectType<SearchFacets>
    {
        protected override void Configure(IObjectTypeDescriptor<SearchFacets> descriptor)
        {
            descriptor
                .Field("country")
                .ResolveWith<SearchFacetResolvers>(x => x.GetCountriesAsync(default!));

            descriptor
                .Field("city")
                .ResolveWith<SearchFacetResolvers>(x => x.GetCitiesAsync(default!));


        }
    }

    public class SearchFacets
    {
    }
}
