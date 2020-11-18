using System;
using System.Collections.Generic;

namespace MagicMedia.Operations
{
    public record RecycleMediaRequest(IEnumerable<Guid> Ids, string OperationId);
}
