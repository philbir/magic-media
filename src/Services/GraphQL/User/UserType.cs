using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor
               .Field("person")
               .ResolveWith<Resolvers>(x => x.GetPersonAsync(default!, default!, default!));

            descriptor
               .Field("permissions")
               .ResolveWith<Resolvers>(x => x.GetPermissions(default!, default!));

            descriptor
               .Field("sharedAlbums")
               .ResolveWith<Resolvers>(x => x.GetSharedAlbumsAsync(default!, default!, default!));

            descriptor
                .Field("inAlbum")
                .ResolveWith<Resolvers>(x => x.GetInAlbumAsync(default!, default!, default!));

            descriptor
               .Field("authorizedOnMedia")
               .ResolveWith<Resolvers>(x => x.GetAuthorizedOnMediaIdsAsync(default!, default!, default!));

            descriptor
               .Field("authorizedOnMediaCount")
               .ResolveWith<Resolvers>(x => x.GetAuthorizedOnMediaCountAsync(default!, default!, default!));
        }

        class Resolvers
        {
            public IEnumerable<string> GetPermissions(
                [Service] IUserService userService,
                [Parent] User user)
            {
                return userService.GetPermissions(user);
            }

            public async Task<Person?> GetPersonAsync(
                [Parent] User user,
                [Service] IPersonService personService,
                CancellationToken cancellationToken)
            {
                if (user.PersonId.HasValue)
                {
                    return await personService.GetByIdAsync(user.PersonId.Value, cancellationToken);
                }

                return null;
            }


            public Task<IEnumerable<Album>> GetSharedAlbumsAsync(
                [Parent] User user,
                [Service] IUserService userService,
                CancellationToken cancellationToken)
            {
                return userService.GetSharedAlbumsAsync(user.Id, cancellationToken);
            }

            public async Task<IEnumerable<Album>> GetInAlbumAsync(
                [Parent] User user,
                [Service] IAlbumService albumService,
                CancellationToken cancellationToken)
            {
                if (user.PersonId.HasValue)
                {
                    return await albumService.GetWithPersonAsync(
                        user.PersonId.Value,
                        cancellationToken);
                }

                return Array.Empty<Album>();
            }

            public async Task<IEnumerable<Guid>> GetAuthorizedOnMediaIdsAsync(
                [Parent] User user,
                [Service] IUserService userService,
                CancellationToken cancellationToken)
            {
                return await userService.GetAuthorizedOnMediaIdsAsync(user.Id, cancellationToken);
            }

            public async Task<int> GetAuthorizedOnMediaCountAsync(
                [Parent] User user,
                [Service] IUserService userService,
                CancellationToken cancellationToken)
            {
                return (await userService.GetAuthorizedOnMediaIdsAsync(user.Id, cancellationToken)).
                    Count();
            }
        }
    }
}
