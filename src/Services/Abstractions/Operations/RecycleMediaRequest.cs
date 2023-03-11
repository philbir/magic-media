using System;
using System.Collections.Generic;
using System.Text;

namespace MagicMedia.Operations;

public record RecycleMediaRequest(IEnumerable<Guid> Ids, string OperationId);

public record DeleteMediaRequest(IEnumerable<Guid> Ids, string OperationId);

public record RescanFacesRequest(IEnumerable<Guid> Ids, string OperationId);

public record ExportMediaRequest(IEnumerable<Guid> Ids, Guid ProfileId, string OperationId)
{
    public string Path { get; init; }
}
