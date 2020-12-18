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
        Task<IEnumerable<Guid>> GetAuthorizedOnAlbumIdsAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetAuthorizedOnMediaIdsAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetAuthorizedOnPersonIdsAsync(Guid userId, CancellationToken cancellationToken);
        IEnumerable<string> GetPermissions(User user);
        Task<IEnumerable<Album>> GetSharedAlbumsAsync(Guid userId, CancellationToken cancellationToken);
        Task<User> TryGetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> TryGetByPersonIdAsync(Guid personId, CancellationToken cancellationToken);
    }
}
