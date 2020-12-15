using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Security
{
    public class NotAuthenticatedUserContext : IUserContext
    {
        private const string _message = "User is not authenticated";

        public bool IsAuthenticated => false;

        public Guid? UserId => null;

        public IEnumerable<string> Roles => new string[0];

        public Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken)
            => throw new UnauthorizedAccessException(_message);

        public Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken)
            => throw new UnauthorizedAccessException(_message);

        public bool HasRole(string role) => throw new UnauthorizedAccessException(_message);

        public Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken)
            => throw new UnauthorizedAccessException(_message);
    }
}
