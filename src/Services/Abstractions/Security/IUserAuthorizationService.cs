using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Security;

public interface IUserAuthorizationService
{
    Task<UserResourceAccessInfo> GetAuthorizedOnAsync(ProtectedResourceType type, CancellationToken cancellationToken);
}
