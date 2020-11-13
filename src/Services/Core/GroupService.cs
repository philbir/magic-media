using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;

namespace MagicMedia
{
    public class GroupService : IGroupService
    {
        private readonly IGroupStore _groupStore;

        public GroupService(IGroupStore groupStore)
        {
            _groupStore = groupStore;
        }

        public async Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _groupStore.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Group>> GetAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _groupStore.GetAsync(ids, cancellationToken);
        }

        public async Task<Group> AddAsync(string name, CancellationToken cancellationToken)
        {
            var group = new Group { Id = Guid.NewGuid(), Name = name };

            await _groupStore.AddAsync(group, cancellationToken);

            return group;
        }
    }
}
