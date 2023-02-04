using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using MagicMedia.Identity.Data;

namespace MagicMedia.Identity.Stores;

public class PersistedGrantStore : IPersistedGrantStore
{
    private readonly IPersistedGrantRepository _repository;

    public PersistedGrantStore(
        IPersistedGrantRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
    {
        filter.Validate();

        return _repository.GetByFilterAsync(filter, default);
    }

    /// <summary>
    /// Gets the grant.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public async Task<PersistedGrant> GetAsync(string key)
    {
        return await _repository
            .GetAsync(key, CancellationToken.None);
    }

    public async Task RemoveAllAsync(PersistedGrantFilter filter)
    {
        filter.Validate();
        await _repository.DeleteByFilterAsync(filter, default);
    }

    /// <summary>
    /// Removes the grant by key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public async Task RemoveAsync(string key)
    {
        await _repository
            .DeleteAsync(key, CancellationToken.None);
    }

    /// <summary>
    /// Stores the grant.
    /// </summary>
    /// <param name="grant">The grant.</param>
    /// <returns></returns>
    public async Task StoreAsync(PersistedGrant grant)
    {
        await _repository
            .SaveAsync(grant, default);
    }
}
