using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Security
{
    public interface IUserContextFactory
    {
        Task<IUserContext> CreateAsync(CancellationToken cancellationToken);
        Task<IUserContext> CreateAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken);
    }
}
