using System;
using System.Collections.Generic;
using MagicMedia.Operations;

namespace MagicMedia.Messaging;

public record RecycleMediaMessage(IEnumerable<Guid> Ids)
{
    public string? OperationId { get; init; }
}

public record DeleteMediaMessage(IEnumerable<Guid> Ids)
{
    public string? OperationId { get; init; }
}


public record RescanFacesMessage(IEnumerable<Guid> Ids)
{
    public string? OperationId { get; init; }
}

public record ExportMediaMessage(IEnumerable<Guid> Ids, Guid ProfileId)
{
    public string? OperationId { get; init; }
    public string? Path { get; init; }
}

public record UpdateMediaMetadataMessage(IEnumerable<Guid> Ids)
{
    public string? OperationId { get; init; }

    public DateTimeOffset? DateTaken { get; init; }

    public UpdateMedataGeoLocation? GeoLocation { get; init; }
}
