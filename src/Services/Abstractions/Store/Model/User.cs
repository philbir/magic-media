using System;
using System.Collections.Generic;

namespace MagicMedia.Store;

public class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public UserState State { get; set; }

    public Guid? PersonId { get; set; }

    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string? InvitationCode { get; set; }
}

public enum UserState
{
    New,
    Invited,
    Active,
    Disabled
}
