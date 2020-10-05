using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Store.MongoDb
{
    public class MongoMediaStore : IMediaStore
    {
        private readonly MediaStoreContext _mediaStoreContext;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public MongoMediaStore(
            MediaStoreContext mediaStoreContext,
            IThumbnailBlobStore thumbnailBlobStore,
            IFaceStore faceStore)
        {
            _mediaStoreContext = mediaStoreContext;
            _thumbnailBlobStore = thumbnailBlobStore;
            Faces = faceStore;
        }

        public IFaceStore Faces { get; }

        public async Task InsertMediaAsync(
            Media media,
            IEnumerable<MediaFace> faces,
            CancellationToken cancellationToken)
        {
            foreach (MediaThumbnail thumb in media.Thumbnails)
            {
                await _thumbnailBlobStore.StoreAsync(
                    new ThumbnailData(thumb.Id, thumb.Data),
                    cancellationToken);

                thumb.Data = null;
            }

            await _mediaStoreContext.Medias.InsertOneAsync(
                media,
                options: null,
                cancellationToken);

            if (faces != null)
            {
                foreach (MediaFace face in faces)
                {
                    await _thumbnailBlobStore.StoreAsync(
                        new ThumbnailData(face.Id, face.Thumnail.Data),
                        cancellationToken);

                    face.Thumnail.Data = null;
                }
            }

            await _mediaStoreContext.Faces.InsertManyAsync(
                faces,
                options: null,
                cancellationToken);
        }
    }
}
