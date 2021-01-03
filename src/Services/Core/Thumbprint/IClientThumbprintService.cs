using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Thumbprint
{
    public interface IClientThumbprintService
    {
        Task<IEnumerable<ClientThumbprint>> GetManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
        Task<string> GetOrCreateAsync(ClientInfo clientInfo, CancellationToken cancellationToken);
    }
}