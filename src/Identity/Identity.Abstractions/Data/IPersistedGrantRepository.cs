using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace MagicMedia.Identity.Data
{
    public interface IPersistedGrantRepository
    {
        /// <summary>
        /// Get entity by id using the specified id property.
        /// </summary>
        /// <param name="key">The entity ids to search for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<PersistedGrant> GetAsync(
            string key,
            CancellationToken cancellationToken);

        Task<PersistedGrant> SaveAsync(PersistedGrant entity,
                                         CancellationToken cancellationToken);

        /// <summary>
        /// Delete entity from collection.
        /// </summary>
        /// <param name="key">The entity key to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task DeleteAsync(
            string key,
            CancellationToken cancellationToken);

        Task DeleteByFilterAsync(
            PersistedGrantFilter filter,
            CancellationToken cancellationToken);

        Task<IEnumerable<PersistedGrant>> GetByFilterAsync(
            PersistedGrantFilter filter,
            CancellationToken cancellationToken);
    }
}
