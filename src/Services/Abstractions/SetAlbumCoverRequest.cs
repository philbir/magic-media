using System;

namespace MagicMedia
{
    public record SetAlbumCoverRequest(Guid AlbumId, Guid MediaId);
}
