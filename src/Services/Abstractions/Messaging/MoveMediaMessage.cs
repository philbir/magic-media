using System;
using System.Collections.Generic;

namespace MagicMedia.Messaging;

public class MoveMediaMessage
{
    public IEnumerable<Guid>? Ids { get; set; }

    public string? NewLocation { get; set; }

    public string? OperationId { get; set; }
}
