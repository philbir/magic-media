using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class MongoMediaStore : IMediaStore
    {
        private readonly MediaStoreContext _mediaStoreContext;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public MongoMediaStore(
            MediaStoreContext mediaStoreContext,
            IThumbnailBlobStore thumbnailBlobStore,
            IFaceStore faceStore,
            ICameraStore cameraStore,
            IPersonStore personStore)
        {
            _mediaStoreContext = mediaStoreContext;
            _thumbnailBlobStore = thumbnailBlobStore;
            Faces = faceStore;
            Cameras = cameraStore;
            Persons = personStore;
        }

        public IFaceStore Faces { get; }

        public ICameraStore Cameras { get; }

        public IPersonStore Persons { get; }

        public async Task<IEnumerable<Media>> SearchAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Media> filter = Builders<Media>.Filter.Empty;

            List<Media> medias = await _mediaStoreContext.Medias.Find(filter)
                .Limit(request.PageSize.GetValueOrDefault(100))
                .ToListAsync(cancellationToken);

            return medias;
        }

        public async Task<Media> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaStoreContext.Medias.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return media;
        }

        public async Task<IReadOnlyDictionary<Guid, MediaThumbnail>> GetThumbnailsByMediaIdsAsync(
            IEnumerable<Guid> mediaIds,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Media> filter = Builders<Media>.Filter
                .In(x => x.Id, mediaIds.ToList());

            filter = filter & Builders<Media>.Filter.ElemMatch(x =>
                x.Thumbnails,
                Builders<MediaThumbnail>.Filter.Eq(t => t.Size, size));

            List<Media> medias = await _mediaStoreContext.Medias.Find(filter)
                .ToListAsync(cancellationToken);

            var result = new Dictionary<Guid, MediaThumbnail>();

            foreach (Media media in medias)
            {
                MediaThumbnail thumb = media.Thumbnails.Where(x => x.Size == size).FirstOrDefault();
                if (thumb != null)
                {
                    thumb.Data = await _thumbnailBlobStore.GetAsync(thumb.Id, cancellationToken);
                    result.Add(media.Id, thumb);
                }
            }

            return result;
        }

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
                        new ThumbnailData(face.Thumbnail.Id, face.Thumbnail.Data),
                        cancellationToken);

                    face.Thumbnail.Data = null;
                }
            }

            if (faces.Any())
            {
                await _mediaStoreContext.Faces.InsertManyAsync(
                    faces,
                    options: null,
                    cancellationToken);
            }
        }
    }
}
