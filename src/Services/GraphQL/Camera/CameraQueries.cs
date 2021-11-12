using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class CameraQueries
    {
        private readonly ICameraService _cameraService;

        public CameraQueries(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public async Task<IEnumerable<Camera>> GetCamerasAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
