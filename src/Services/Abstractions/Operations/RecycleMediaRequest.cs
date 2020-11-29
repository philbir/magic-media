using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MagicMedia.Operations
{
    public record RecycleMediaRequest(IEnumerable<Guid> Ids, string OperationId);

    public record DeleteMediaRequest(IEnumerable<Guid> Ids, string OperationId);

    public record RescanFacesRequest(IEnumerable<Guid> Ids, string OperationId);
}
