using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Data
{
    public interface IApiResourceRepository
    {
        Task<IEnumerable<MagicApiResource>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<MagicApiResource>> GetByNameAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken);

        Task<IEnumerable<MagicApiResource>> GetByScopeNameAsync(
            IEnumerable<string> scopeNames,
            CancellationToken cancellationToken);
    }
}
