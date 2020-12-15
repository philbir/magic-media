using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Security
{
    public class DefaultUserContext : IUserContext
    {
        private readonly User _user;

        public DefaultUserContext(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
        }

        public bool IsAuthenticated => true;

        public Guid? UserId => _user.Id;

        public IEnumerable<string> Roles => _user.Roles;

        public Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken)
        {
            return new List<Guid> { Guid.NewGuid() };
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role, StringComparer.InvariantCulture);
        }

        public async Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken)
        {
            switch (type)
            {
                case ProtectedResourceType.Media:
                    IEnumerable<Guid> ids = await GetAuthorizedMediaAsync(cancellationToken);
                    return ids.Contains((Guid)resourceId);
            }

            return false;
        }

    }

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
