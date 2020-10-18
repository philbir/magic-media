using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Data.Common;

namespace MagicMedia.Store.MongoDb
{
    public class CameraStore : ICameraStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public CameraStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<Camera> TryGetAsync(
            string make,
            string model,
            CancellationToken cancellationToken)
        {
            Camera cam = await _mediaStoreContext.Cameras.AsQueryable()
                .Where(x => x.Make == make && x.Model == model)
                .FirstOrDefaultAsync(cancellationToken);

            return cam;
        }

        public async Task<Camera> GetAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Camera cam = await _mediaStoreContext.Cameras.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return cam;
        }

        public async Task<IEnumerable<Camera>> GetManyAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            List<Camera> cams = await _mediaStoreContext.Cameras.AsQueryable()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return cams;
        }

        public async Task<Camera> CreateAsync(Camera camera, CancellationToken cancellationToken)
        {
            if (camera.Id == Guid.Empty)
            {
                camera.Id = Guid.NewGuid();
            }

            await _mediaStoreContext.Cameras.InsertOneAsync(
                camera,
                options: null,
                cancellationToken);

            return camera;
        }
    }
}
