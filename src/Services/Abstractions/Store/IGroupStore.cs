using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store.MongoDb
{
    public interface IGroupStore
    {
        Task AddAsync(Group group, CancellationToken cancellationToken);
        Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Group>> GetAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}