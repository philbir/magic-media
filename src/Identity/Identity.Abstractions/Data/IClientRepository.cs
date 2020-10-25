using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Identity.Data
{
    public interface IClientRepository
    {
        Task<MagicClient> GetAsync(
            string id,
            CancellationToken cancellationToken);

        Task<HashSet<string>> GetAllClientOrigins();

        Task<HashSet<string>> GetAllClientRedirectUriAsync();
    }
}
