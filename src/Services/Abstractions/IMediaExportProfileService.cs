using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia;

public interface IMediaExportProfileService
{
    Task<MediaExportProfile> GetDefaultProfile(CancellationToken cancellationToken);
    Task<MediaExportProfile> GetProfileOrDefault(Guid? profileId, CancellationToken cancellationToken);
    Task<IEnumerable<MediaExportProfile>> GetAllAsync(CancellationToken cancellationToken);
}
