using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store;

public interface IClientThumbprintStore
{
    Task AddAsync(ClientThumbprint clientThumbprint, CancellationToken cancellationToken);
    Task<IEnumerable<ClientThumbprint>> GetManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
    Task<ClientThumbprint> TryGetByIdAsync(string id, CancellationToken cancellationToken);
}
