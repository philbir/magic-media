using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IDuplicateMediaGuard
{
    Task InitializeAsync(CancellationToken cancellationToken);
    bool IsDuplicate(IEnumerable<MediaHash> hashes);
    void AddMedia(IEnumerable<MediaHash> hashes);
}
