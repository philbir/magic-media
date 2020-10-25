using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Store;
using MagicMedia.TestLibrary;
using MagicMedia.Thumbnail;

namespace MagicMedia.Playground
{
    public class ImportSample
    {
        private readonly IMediaStore _store;
        private readonly IMetadataExtractor _metadataExtractor;
        private readonly IFaceDetectionService _faceDetectionService;
        private readonly IBoxExtractorService _faceExtractorService;
        private readonly IThumbnailService _thumbnailService;

        public ImportSample(
            IMediaStore store,
            IMetadataExtractor metadataExtractor,
            IFaceDetectionService faceDetectionService,
            IBoxExtractorService faceExtractorService,
            IThumbnailService thumbnailService)
        {
            _store = store;
            _metadataExtractor = metadataExtractor;
            _faceDetectionService = faceDetectionService;
            _faceExtractorService = faceExtractorService;
            _thumbnailService = thumbnailService;
        }

        public async Task ImportMedia()
        {
            Stream image = TestMediaLibrary.TwoFacesWithExif;

            MediaMetadata meta = await _metadataExtractor.GetMetadataAsync(image, default);

            image.Position = 0;
            IEnumerable<FaceData> dataData = await GetFaceData(image);

            image.Position = 0;

            IEnumerable<ThumbnailResult> thums = await _thumbnailService
                .GenerateAllThumbnailAsync(image, default);

            var media = new Media
            {
                Id = Guid.NewGuid(),
                MediaType = MediaType.Image,
                DateTaken = meta.DateTaken,
                GeoLocation = meta.GeoLocation,
                Size = image.Length
            };

            media.Thumbnails = thums.Select(x => new MediaThumbnail
            {
                Id = Guid.NewGuid(),
                Data = x.Data,
                Format = x.Format,
                Dimensions = x.Dimensions,
                Size = x.Size
            });


            IEnumerable<MediaFace> faces = dataData.Select(f => new MediaFace
            {
                Box = f.Box,
                Id = f.Id,
                Encoding = f.Encoding,
                MediaId = media.Id,
                RecognitionType = FaceRecognitionType.None,
                State = FaceState.New,
                Thumbnail = f.Thumbnail,
            });

            media.FaceCount = faces.Count();

            await _store.InsertMediaAsync(media, faces, default);
        }

        private async Task<IEnumerable<FaceData>> GetFaceData(Stream image)
        {
            IEnumerable<FaceDetectionResponse> detectedFaces = await _faceDetectionService
                .DetectFacesAsync(image, default);

            IEnumerable<FaceData> faceData = detectedFaces.Select(f =>
                new FaceData
                {
                    Box = f.Box,
                    Encoding = f.Encoding,
                    Id = Guid.NewGuid()
                }).ToList();

            IEnumerable<BoxExtractionInput> inputs = faceData.Select(f =>
                new BoxExtractionInput
                {
                    Box = f.Box,
                    Id = f.Id
                });

            image.Position = 0;

            IEnumerable<BoxExtractionResult> faceImages = await _faceExtractorService
                .ExtractBoxesAsync(image, inputs, ThumbnailSizeName.M, default);

            ILookup<Guid, BoxExtractionResult> faceLookup = faceImages.ToLookup(x => x.Id);

            foreach (FaceData face in faceData)
            {
                ThumbnailResult thumb = faceLookup[face.Id].Single().Thumbnail;

                face.Thumbnail = new MediaThumbnail
                {
                    Id = Guid.NewGuid(),
                    Data = thumb.Data,
                    Dimensions = thumb.Dimensions,
                    Format = thumb.Format,
                    Size = thumb.Size
                };
            }

            return faceData;
        }
    }
}
