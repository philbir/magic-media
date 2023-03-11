using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store;

public interface IMediaExportProfileStore
{
    Task<IEnumerable<MediaExportProfile>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<MediaExportProfile> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddAsync(MediaExportProfile profile, CancellationToken cancellationToken);

    Task<MediaExportProfile> GetDefaultAsync(
        CancellationToken cancellationToken);
}
