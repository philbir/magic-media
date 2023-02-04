using GreenDonut;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.DataLoaders;

public class UserByIdDataLoader : BatchDataLoader<Guid, User>
{
    private readonly IUserService _userService;

    public UserByIdDataLoader(
        IBatchScheduler batchScheduler,
        IUserService userService)
            : base(batchScheduler)
    {
        _userService = userService;
    }

    protected override async Task<IReadOnlyDictionary<Guid, User>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _userService.GetManyAsync(keys, cancellationToken);

        return users.ToDictionary(x => x.Id);
    }
}
