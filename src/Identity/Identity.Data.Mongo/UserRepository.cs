using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MagicMedia.Identity.Data.Mongo
{
    public class UserRepository : IUserRepository
    {
        private readonly IIdentityDbContext _loginDbContext;

        public UserRepository(IIdentityDbContext loginDbContext)
        {
            _loginDbContext = loginDbContext;
        }

        public async Task<User?> TryGetUserByProvider(
            string provider,
            string userIdentifier,
            CancellationToken cancellationToken)
        {

            FilterDefinition<UserAuthProvider> providerFilter =
                Builders<UserAuthProvider>.Filter.And(
                Builders<UserAuthProvider>.Filter.Eq(
                    x => x.Name, provider),
                Builders<UserAuthProvider>.Filter.Eq(
                    x => x.UserIdentifier, userIdentifier)
                );

            FilterDefinition<User> filter = Builders<User>.Filter
                .ElemMatch(x => x.AuthProviders, providerFilter);

            IAsyncCursor<User> cursor = await _loginDbContext.Users.FindAsync(filter, null, cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
        {
            await _loginDbContext.Users.InsertOneAsync(
                user,
                options: null,
                cancellationToken);

            return user;
        }
    }
}
