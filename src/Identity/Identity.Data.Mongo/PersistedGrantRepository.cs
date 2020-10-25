using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Driver;

namespace MagicMedia.Identity.Data.Mongo
{
    public class PersistedGrantRepository : IPersistedGrantRepository
    {
        private readonly IIdentityDbContext _loginDbContext;

        public PersistedGrantRepository(IIdentityDbContext loginDbContext)
        {
            _loginDbContext = loginDbContext;
        }

        public async Task<PersistedGrant> GetAsync(
            string key,
            CancellationToken cancellationToken)
        {
            FilterDefinition<PersistedGrant> findFilter =
                Builders<PersistedGrant>.Filter.Eq(grant => grant.Key, key);

            return (await _loginDbContext.PersistedGrants
                    .FindAsync(findFilter, cancellationToken: cancellationToken)
                    .ConfigureAwait(false))
                .ToList()
                .SingleOrDefault();
        }

        public async Task<IEnumerable<PersistedGrant>> GetByFilterAsync(
            PersistedGrantFilter filter,
            CancellationToken cancellationToken)
        {
            FilterDefinition<PersistedGrant>? dbFilter = GetFilter(filter);

            return (await _loginDbContext.PersistedGrants
                    .FindAsync(dbFilter, cancellationToken: cancellationToken)
                    .ConfigureAwait(false))
                .ToList();
        }

        public async Task<PersistedGrant> SaveAsync(
            PersistedGrant entity,
            CancellationToken cancellationToken)
        {
            await _loginDbContext.PersistedGrants
                        .ReplaceOneAsync(
                            x => x.Key == entity.Key,
                            entity,
                            options: new ReplaceOptions { IsUpsert = true },
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            return entity;
        }

        public async Task DeleteAsync(
            string key,
            CancellationToken cancellationToken)
        {
            FilterDefinition<PersistedGrant> findFilter =
                Builders<PersistedGrant>.Filter.Eq(grant => grant.Key, key);

            await _loginDbContext.PersistedGrants
                .DeleteOneAsync(findFilter, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteByFilterAsync(
            PersistedGrantFilter filter,
            CancellationToken cancellationToken)
        {
            FilterDefinition<PersistedGrant>? dbFilter = GetFilter(filter);

            await _loginDbContext.PersistedGrants.DeleteManyAsync(dbFilter, cancellationToken);
        }

        private FilterDefinition<PersistedGrant> GetFilter(PersistedGrantFilter filter)
        {
            FilterDefinition<PersistedGrant> query = Builders<PersistedGrant>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query & Builders<PersistedGrant>.Filter
                    .Eq(x => x.ClientId, filter.ClientId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query & Builders<PersistedGrant>.Filter
                    .Eq(x => x.SessionId, filter.SessionId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query & Builders<PersistedGrant>.Filter
                    .Eq(x => x.SubjectId, filter.SubjectId);
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query & Builders<PersistedGrant>.Filter
                    .Eq(x => x.Type, filter.Type);
            }

            return query;
        }
    }
}

