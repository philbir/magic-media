using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IAlbumMediaIdResolver
{
    Task<IEnumerable<Guid>> GetMediaIdsAsync(Album album, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> GetMediaIdsAsync(Guid id, CancellationToken cancellationToken);
}
