using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IGroupService
{
    Task<Group> AddAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Group>> GetAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
