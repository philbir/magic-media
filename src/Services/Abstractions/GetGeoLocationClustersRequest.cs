using System;
using System.Collections.Generic;

namespace MagicMedia;

public record GetGeoLocationClustersRequest(GeoBox Box, int Precision)
{
    public IEnumerable<Guid>? AuthorizedOn { get; init; }
}
