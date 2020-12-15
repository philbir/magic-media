using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Security
{
    public interface IUserService
    {
        Task<User> CreateFromPersonAsync(CreateUserFromPersonRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<User> TryGetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> TryGetByPersonIdAsync(Guid personId, CancellationToken cancellationToken);
    }
}
