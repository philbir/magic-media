using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class PersonType :  ObjectType<Person>
    {
        protected override void Configure(IObjectTypeDescriptor<Person> descriptor)
        {
            descriptor
                .Field("groups")
                .ResolveWith<PersonResolvers>(x => x.GetGroupsAsync(default!, default!));

            descriptor
                .Field("thumbnail")
                .ResolveWith<PersonResolvers>(x => x.GetThumbnailAsync(default!, default!));

            descriptor
                .Field("timeline")
                .Argument("itemsPerYear", a => a
                    .DefaultValue(5)
                    .Type(typeof(int?)))
                .ResolveWith<PersonResolvers>(x => x.GetTimelineAsync(default!, default!, default!));
        }
    }
}
