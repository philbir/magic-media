using System;
using System.Collections.Generic;

namespace MagicMedia.Messaging
{
    public record RecycleMediaMessage(IEnumerable<Guid> Ids)
    {
        public string? OperationId { get; init; }
    }

}
