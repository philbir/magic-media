using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;

namespace MagicMedia
{
    public class WorkerUserContextFactory : IUserContextFactory
    {
        public Task<IUserContext> CreateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IUserContext) new WorkerUserContext());
        }

        public Task<IUserContext> CreateAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken)
        {
            return Task.FromResult((IUserContext) new WorkerUserContext());
        }
    }

    public class WorkerUserContext : IUserContext
    {
        public bool IsAuthenticated => true;

        public Guid? UserId => Guid.Empty;

        public IEnumerable<string> Roles => new List<string>();

        private IEnumerable<Guid> _emptyList = new List<Guid>();

        public Task<IEnumerable<Guid>> GetAuthorizedAlbumAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_emptyList);
        }

        public Task<IEnumerable<Guid>> GetAuthorizedFaceAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_emptyList);
        }

        public Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_emptyList);
        }

        public Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_emptyList);
        }

        public bool HasPermission(string permission)
        {
            return true;
        }

        public bool HasRole(string role)
        {
            return true;
        }

        public Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
