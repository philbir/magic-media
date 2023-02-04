using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL;

public class PersonType : ObjectType<Person>
{
    protected override void Configure(IObjectTypeDescriptor<Person> descriptor)
    {
        descriptor
            .Field("groups")
            .ResolveWith<Resolvers>(x => x.GetGroupsAsync(default!, default!, default!));

        descriptor
            .Field("thumbnail")
            .ResolveWith<Resolvers>(x => x.GetThumbnailAsync(default!, default!, default!))
            .Use(next => async context =>
            {
                await next(context);
                var thumb = context.Result;
            });

        descriptor
            .Field("inAlbum")
            .ResolveWith<Resolvers>(x => x.GetInAlbumAsync(default!, default!, default!));

        descriptor
            .Field("user")
            .ResolveWith<Resolvers>(x => x.GetUserAsync(default!, default!, default!));

        descriptor
            .Field("timeline")
            .Argument("itemsPerYear", a => a
                .DefaultValue(5)
                .Type(typeof(int?)))
            .ResolveWith<Resolvers>(x => x.GetTimelineAsync(default!, default!, default!, default!));
    }

    class Resolvers
    {
        public Task<User?> GetUserAsync(
            [Service] IUserService userService,
            [Parent] Person person,
            CancellationToken cancellationToken)
                => userService.TryGetByPersonIdAsync(person.Id, cancellationToken);

        public async Task<IEnumerable<Group>> GetGroupsAsync(
            [Service] IGroupService groupService,
            [Parent] Person person,
            CancellationToken cancellationToken)
        {
            if (person.Groups is { } groups && groups.Any())
            {
                return await groupService.GetAsync(person.Groups, cancellationToken);
            }

            return new Group[0];
        }

        public Task<MediaThumbnail?> GetThumbnailAsync(
            [Service] IPersonService personService,
            [Parent] Person person,
            CancellationToken cancellationToken)
                => personService.TryGetFaceThumbnailAsync(
                person.Id,
                cancellationToken);

        public Task<PersonTimeline> GetTimelineAsync(
            [Service] IPersonTimelineService personTimelineService,
            [Parent] Person person,
            int itemsPerYear,
            CancellationToken cancellationToken)
                => personTimelineService.BuildTimelineAsync(
                        person.Id,
                        itemsPerYear,
                        cancellationToken);

        public Task<IEnumerable<Album>> GetInAlbumAsync(
            [Service] IAlbumService albumService,
            [Parent] Person person,
            CancellationToken cancellationToken)
                => albumService.GetWithPersonAsync(
                    person.Id,
                    cancellationToken);
    }
}
