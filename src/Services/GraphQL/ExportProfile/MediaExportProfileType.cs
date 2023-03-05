using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL;

public class MediaExportProfileType : ObjectType<MediaExportProfile>
{
    protected override void Configure(IObjectTypeDescriptor<MediaExportProfile> descriptor)
    {
        descriptor
            .Field("isUserCurrent")
            .ResolveWith<Resvolvers>(x => x.GetIsUserCurrentAsync(default!, default!, default!, default!));
    }

    class Resvolvers
    {
        public async Task<bool> GetIsUserCurrentAsync(
            [Parent] MediaExportProfile profile,
            [Service] IUserContextFactory userContextFactory,
            [DataLoader] UserByIdDataLoader userByIdDataLoader,
            CancellationToken cancellationToken)
        {
            IUserContext userContext = await userContextFactory.CreateAsync(cancellationToken);

            IReadOnlyList<User> user = await userByIdDataLoader.LoadAsync(
                new[] { userContext.UserId.Value },
                cancellationToken);

            return user.Single().CurrentExportProfile is Guid id && profile.Id == id;
        }
    }
}
