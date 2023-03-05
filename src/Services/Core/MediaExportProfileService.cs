using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia;

public class MediaExportProfileService : IMediaExportProfileService
{
    private readonly IUserContextFactory _userContextFactory;
    private readonly IMediaExportProfileStore _store;
    private readonly IUserService _userService;

    public MediaExportProfileService(
        IUserContextFactory userContextFactory,
        IMediaExportProfileStore store,
        IUserService userService)
    {
        _userContextFactory = userContextFactory;
        _store = store;
        _userService = userService;
    }

    public async Task<MediaExportProfile> GetDefaultProfile(CancellationToken cancellationToken)
    {
        IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);
        User user = await _userService.GetByIdAsync(userContext.UserId.Value, cancellationToken);

        if (user.CurrentExportProfile.HasValue)
        {
            return await _store.GetByIdAsync(user.CurrentExportProfile.Value, cancellationToken);
        }

        return await _store.GetDefaultAsync(cancellationToken);
    }

    public Task<IEnumerable<MediaExportProfile>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _store.GetAllAsync(cancellationToken);
    }

    public async Task<MediaExportProfile> GetProfileOrDefault(Guid? profileId, CancellationToken cancellationToken)
    {
        if (profileId is null)
        {
            return await GetDefaultProfile(cancellationToken);
        }
        else
        {
            return await _store.GetByIdAsync(profileId.Value, cancellationToken);
        }
    }
}
