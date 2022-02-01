using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia;

public interface IVideoPlayerService
{
    Task<MediaStream> GetVideoAsync(Guid id, CancellationToken cancellationToken);
    MediaStream GetVideoPreview(Guid id, CancellationToken cancellationToken);
}
