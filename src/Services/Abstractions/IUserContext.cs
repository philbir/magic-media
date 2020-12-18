using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IUserContext
    {
        public bool IsAuthenticated { get;}

        Guid? UserId { get; }

        IEnumerable<string> Roles { get; }

        bool HasRole(string role);

        bool HasPermission(string permission);

        Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken);

        Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetAuthorizedAlbumAsync(CancellationToken cancellationToken);
    }


    public enum ProtectedResourceType
    {
        Media,
        Person,
        Album
    }

    public class UserResourceAccessInfo
    {
        public ProtectedResourceType Type { get; set; }

        public bool ViewAll { get; set; }

        public IEnumerable<Guid> Ids { get; set; }
    }
}
