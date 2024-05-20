using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing;

public interface IMediaSourcePreConverter
{
    Task PreConvertAsync(
        CancellationToken cancellationToken);
}
