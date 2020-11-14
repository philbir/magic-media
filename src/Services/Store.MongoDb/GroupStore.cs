using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class GroupStore : IGroupStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public GroupStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Groups.AsQueryable()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Group>> GetAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Groups.AsQueryable()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Group group, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Groups.InsertOneAsync(
                group,
                options: null,
                cancellationToken);
        }
    }
}
