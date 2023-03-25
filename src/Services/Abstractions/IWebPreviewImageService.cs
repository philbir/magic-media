using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IWebPreviewImageService
{
    Task SavePreviewImageAsync(Media media, CancellationToken cancellationToken);
}
