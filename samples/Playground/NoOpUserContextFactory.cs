using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Security;

namespace MagicMedia.Playground
{
    public class NoOpUserContextFactory : IUserContextFactory
    {
        public Task<IUserContext> CreateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IUserContext)new NoOpUserContext());
        }

        public Task<IUserContext> CreateAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            return CreateAsync(cancellationToken);
        }
    }


    public class NoOpUserContext : IUserContext
    {
        public bool IsAuthenticated => throw new NotImplementedException();

        public Guid? UserId => throw new NotImplementedException();

        public IEnumerable<string> Roles => throw new NotImplementedException();

        public Task<IEnumerable<Guid>> GetAuthorizedAlbumAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetAuthorizedFaceAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public ClientInfo GetClientInfo()
        {
            throw new NotImplementedException();
        }

        public bool HasPermission(string permission)
        {
            throw new NotImplementedException();
        }

        public bool HasRole(string role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
