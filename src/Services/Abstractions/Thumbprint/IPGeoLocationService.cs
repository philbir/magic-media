using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Thumbprint
{
    public interface IPGeoLocationService
    {
        Task<GeoIpLocation> LookupAsync(string ipAddress, CancellationToken cancellationToken);
    }
}