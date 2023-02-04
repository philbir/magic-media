using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store;

public interface ICameraStore
{
    Task<Camera> CreateAsync(Camera camera, CancellationToken cancellationToken);
    Task<Camera> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Camera>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<Camera> TryGetAsync(string make, string model, CancellationToken cancellationToken);
}
