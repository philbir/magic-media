using System;
using System.Collections.Generic;

namespace MagicMedia.Identity.Data;

public class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public IEnumerable<UserAuthProvider>? AuthProviders { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLogin { get; set; }

    public UserStatus Status { get; set; }
}

public enum UserStatus
{
    Active,
    Disabled
}

public class UserAuthProvider
{
    public string? Name { get; set; }

    public string? UserIdentifier { get; set; }

}
