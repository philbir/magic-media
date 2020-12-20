using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
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
