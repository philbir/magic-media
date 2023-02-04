using System;
using System.Collections.Generic;

namespace MagicMedia;

public class UpdatePersonRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTimeOffset? DateOfBirth { get; set; }

    public IEnumerable<Guid>? Groups { get; set; }

    public Guid? ProfileFaceId { get; set; }

    public IEnumerable<string>? NewGroups { get; set; }
}
