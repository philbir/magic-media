using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb;

public class MediaExportProfileStore : IMediaExportProfileStore
{
    private readonly MediaStoreContext _mediaStoreContext;

    public MediaExportProfileStore(MediaStoreContext mediaStoreContext)
    {
        _mediaStoreContext = mediaStoreContext;
    }

    public async Task<IEnumerable<MediaExportProfile>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _mediaStoreContext.MediaExportProfile.AsQueryable()
            .ToListAsync(cancellationToken);
    }

    public async Task<MediaExportProfile> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _mediaStoreContext.MediaExportProfile.AsQueryable()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<MediaExportProfile> GetDefaultAsync(
        CancellationToken cancellationToken)
    {
        MediaExportProfile? profile = await _mediaStoreContext.MediaExportProfile.AsQueryable()
            .Where(x => x.IsDefault)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile is null)
        {
            profile = await _mediaStoreContext.MediaExportProfile.AsQueryable()
                .FirstOrDefaultAsync(cancellationToken);
        }

        return profile;
    }

    public async Task AddAsync(MediaExportProfile profile, CancellationToken cancellationToken)
    {
        await _mediaStoreContext.MediaExportProfile.InsertOneAsync(
            profile,
            options: null,
            cancellationToken);
    }
}
