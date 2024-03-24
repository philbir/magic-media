using System;
using System.Collections.Generic;

namespace MagicMedia.Store;

public class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public UserState State { get; set; }

    public IEnumerable<UserIdentifier> Identifiers { get; set; } = new List<UserIdentifier>();

    public Guid? PersonId { get; set; }

    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string? InvitationCode { get; set; }
    public Guid? CurrentExportProfile { get; set; }
}

public class UserIdentifier
{
    public string Method { get; set; }

    public string Value { get; set; }
}

public enum UserState
{
    New,
    Invited,
    Active,
    Disabled
}
