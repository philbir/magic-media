using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public class CameraService : ICameraService
{
    private readonly ICameraStore _cameraStore;

    public CameraService(ICameraStore cameraStore)
    {
        _cameraStore = cameraStore;
    }

    public async Task<Camera> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _cameraStore.GetAsync(id, cancellationToken);
    }

    public async Task<Camera> GetOrCreateAsync(
        string make,
        string model,
        CancellationToken cancellationToken)
    {
        Camera? camera = await _cameraStore.TryGetAsync(make, model, cancellationToken);

        if (camera == null)
        {
            camera = await _cameraStore.CreateAsync(
                new Camera { Id = Guid.NewGuid(), Make = make, Model = model }, cancellationToken);
        }

        return camera;
    }
}
