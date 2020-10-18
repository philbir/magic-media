using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia
{
    public interface ICameraService
    {
        Task<Camera> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Camera> GetOrCreateAsync(string make, string model, CancellationToken cancellationToken);
    }
}