using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Data;

public interface IUserRepository
{
    Task<User> AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> TryGetUserByProvider(
        string provider,
        string userIdentifier,
        CancellationToken cancellationToken);
}
