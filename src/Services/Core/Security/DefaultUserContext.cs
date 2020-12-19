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
        private readonly IUserService _userService;
        private HashSet<string> _allPermissions = new();

        public DefaultUserContext(User user, IUserService userService)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userService = userService;
            SetAllPermissions();
        }

        private void SetAllPermissions()
        {
            _allPermissions = new HashSet<string>(_userService.GetPermissions(_user));
        }

        public bool IsAuthenticated => true;

        public Guid? UserId => _user.Id;

        public IEnumerable<string> Roles => _user.Roles;

        public async Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(
            CancellationToken cancellationToken)
        {
            return await _userService.GetAuthorizedOnPersonIdsAsync(UserId!.Value, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken)
        {
            return await _userService.GetAuthorizedOnMediaIdsAsync(UserId!.Value, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedFaceAsync(CancellationToken cancellationToken)
        {
            return await _userService.GetAuthorizedOnFaceIdsAsync(UserId!.Value, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetAuthorizedAlbumAsync(CancellationToken cancellationToken)
        {
            return await _userService.GetAuthorizedOnAlbumIdsAsync(UserId!.Value, cancellationToken);
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role, StringComparer.InvariantCulture);
        }

        public async Task<bool> IsAuthorizedAsync(
            object resourceId,
            ProtectedResourceType type,
            CancellationToken cancellationToken)
        {
            switch (type)
            {
                case ProtectedResourceType.Media:
                    IEnumerable<Guid> ids = await GetAuthorizedMediaAsync(cancellationToken);
                    return ids.Contains((Guid)resourceId);
                case ProtectedResourceType.Face:
                    IEnumerable<Guid> faceIds = await GetAuthorizedFaceAsync(cancellationToken);
                    return faceIds.Contains((Guid)resourceId);
                case ProtectedResourceType.Album:
                    IEnumerable<Guid> albumIds = await GetAuthorizedAlbumAsync(cancellationToken);
                    return albumIds.Contains((Guid)resourceId);
            }

            return false;
        }

        public bool HasPermission(string permission)
        {
            return _allPermissions.Contains(permission);
        }
    }
}
