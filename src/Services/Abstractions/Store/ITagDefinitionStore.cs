using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store;

public interface ITagDefinitionStore
{
    Task<IReadOnlyList<TagDefintion>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TagDefintion>> GetManyAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);
}
