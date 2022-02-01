using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Data;

public interface IIdentityResourceRepository
{
    Task<IEnumerable<MagicIdentityResource>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IEnumerable<MagicIdentityResource>> GetByNameAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken);
}
