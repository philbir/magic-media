using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint
{
    public interface IGeoIPLocationService
    {
        Task<GeoIpLocation> LookupAsync(string ipAddress, CancellationToken cancellationToken);
    }
}