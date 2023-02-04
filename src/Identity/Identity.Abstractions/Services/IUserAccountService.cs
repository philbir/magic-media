using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Services;

public interface IUserAccountService
{
    Task<AuthenticateUserResult> AuthenticateExternalUserAsync(AuthenticateExternalUserRequest request, CancellationToken cancellationToken);
}
