using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia
{
    public interface IGeoDecoderService
    {
        Task<GeoAddress> DecodeAsync(
            double latitude, 
            double longitude, 
            CancellationToken cancellationToken);
    }
}
