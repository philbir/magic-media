using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;

namespace MagicMedia.Identity.Data
{
    public interface IApiScopeRepository
    {
        Task<IEnumerable<ApiScope>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<ApiScope>> GetByNameAsync(
            IEnumerable<string> scopeNames,
            CancellationToken cancellationToken);
    }
}
