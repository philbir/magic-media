using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;

namespace MagicMedia.Store.MongoDb
{
    public interface IUserStore
    {
        Task<User> AddAsync(User user, CancellationToken cancellationToken);
        Task<User> AddUpdateAsync(User user, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<User> TryGetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> TryGetByPersonIdAsync(Guid personId, CancellationToken cancellationToken);
        Task<SearchResult<User>> SearchAsync(SearchUserRequest request, CancellationToken cancellationToken);
    }
}
