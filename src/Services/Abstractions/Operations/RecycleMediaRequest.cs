using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MagicMedia.Operations
{
    public record RecycleMediaRequest(IEnumerable<Guid> Ids, string OperationId);
}
