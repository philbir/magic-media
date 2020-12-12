using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    internal class MediaResolvers
    {
        private readonly IMediaStore _mediaStore;
        private readonly IFaceService _faceService;
        private readonly IMediaService _mediaService;

        public MediaResolvers(
            IMediaStore mediaStore,
            IFaceService faceService,
            IMediaService mediaService)
        {
            _mediaStore = mediaStore;
            _faceService = faceService;
            _mediaService = mediaService;
        }

        public async Task<Camera?> GetCameraAsync(
            Media media,
            CameraByIdDataLoader cameraById,
            CancellationToken cancellationToken)
        {
            if (media.CameraId.HasValue)
            {
                return await cameraById.LoadAsync(media.CameraId.Value, cancellationToken);
            }

            return null;
        }

        public async Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
            Media media,
            CancellationToken cancellationToken)
        {
            return await _faceService.GetFacesByMediaAsync(media.Id, cancellationToken);
        }

        public IEnumerable<MediaFileInfo> GetFileInfos(
            Media media)
        {
            return _mediaService.GetMediaFiles(media);
        }

        public async Task<MediaAI> GetAIDataAsync(
            Media media,
            double? minConfidence=null,
            //AISource? source=null,
            CancellationToken cancellationToken=default)
        {
            MediaAI ai = await _mediaService.GetAIDataAsync(media.Id, cancellationToken);

            if ( ai == null)
            {
                return new MediaAI
                {
                    Id = Guid.Empty,
                    MediaId = media.Id,
                    Objects = new List<MediaAIObject>(),
                    Tags = new List<MediaAITag>()
                };
            }

            if (minConfidence.HasValue)
            {
                ai.Objects = ai.Objects.Where(x => x.Confidence >= minConfidence);
                ai.Tags = ai.Tags.Where(x => x.Confidence >= minConfidence);
            }

            //if (source.HasValue)
            //{
            //    ai.Objects = ai.Objects.Where(x => x.Source == source);
            //    ai.Tags = ai.Tags.Where(x => x.Source == source);
            //}

            ai.Objects = ai.Objects.OrderByDescending(x => x.Confidence);
            ai.Tags = ai.Tags.OrderByDescending(x => x.Confidence);

            return ai;
        }
    }
}
