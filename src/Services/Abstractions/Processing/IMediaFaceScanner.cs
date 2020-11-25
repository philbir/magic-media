using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing
{
    public interface IMediaFaceScanner
    {
        Task ScanByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken);
    }
}