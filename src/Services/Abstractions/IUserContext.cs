using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia;

public interface IUserContext
{
    public bool IsAuthenticated { get; }

    Guid? UserId { get; }

    IEnumerable<string> Roles { get; }

    bool HasRole(string role);

    bool HasPermission(string permission);

    ClientInfo GetClientInfo();

    Task<IEnumerable<Guid>> GetAuthorizedPersonsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Guid>> GetAuthorizedMediaAsync(CancellationToken cancellationToken);

    Task<bool> IsAuthorizedAsync(object resourceId, ProtectedResourceType type, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetAuthorizedAlbumAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetAuthorizedFaceAsync(CancellationToken cancellationToken);
}

public class ClientInfo
{
    public string? IPAdddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Request { get; set; }
}

public interface IUserClientInfoResolver
{
    public ClientInfo Resolve();
}

public enum ProtectedResourceType
{
    Media,
    Face,
    Person,
    Album,
    User
}

public class ResourceInfo
{
    public string? Source { get; set; }

    public object? Id { get; set; }

    public string? Raw { get; set; }

    public ProtectedResourceType? Type { get; set; }
}

public class UserResourceAccessInfo
{
    public ProtectedResourceType Type { get; set; }

    public bool ViewAll { get; set; }

    public IEnumerable<Guid>? Ids { get; set; }
}
