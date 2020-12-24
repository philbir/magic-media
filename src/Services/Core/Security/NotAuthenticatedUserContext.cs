using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Security
{
    public class NotAuthenticatedUserContext : IUserContext
    {
        private const string _message = "User is not authenticated";

        private IEnumerable<Guid> _emptyList = new List<Guid>();

        public bool IsAuthenticated => false;

        public Guid? UserId => null;

        public IEnumerable<string> Roles => new string[0];

        public Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken)
            => Task.FromResult(_emptyList);

        public Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken)
            => Task.FromResult(_emptyList);

        public bool HasRole(string role) => throw new UnauthorizedAccessException(_message);

        public Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public bool HasPermission(string permission)
            => false;

        public Task<IEnumerable<Guid>> GetAuthorizedAlbumAsync(CancellationToken cancellationToken)
            => Task.FromResult(_emptyList);

        public Task<IEnumerable<Guid>> GetAuthorizedFaceAsync(CancellationToken cancellationToken)
           => Task.FromResult(_emptyList);

        public ClientInfo GetClientInfo()
        {
            return new ClientInfo();
        }
    }
}
