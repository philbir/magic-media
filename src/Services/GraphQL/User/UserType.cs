using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor
               .Field("person")
               .ResolveWith<UserResolvers>(x => x.GetPersonAsync(default!, default!));

            descriptor
               .Field("sharedAlbums")
               .ResolveWith<UserResolvers>(x => x.GetSharedAlbumsAsync(default!, default!));

            descriptor
               .Field("authorizedOnMedia")
               .ResolveWith<UserResolvers>(x => x.GetAuthorizedOnMediaIdsAsync(default!, default!));

            descriptor
               .Field("authorizedOnMediaCount")
               .ResolveWith<UserResolvers>(x => x.GetAuthorizedOnMediaCountAsync(default!, default!));
        }
    }
}
