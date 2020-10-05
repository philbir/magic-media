using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.Extensions;
using MagicMedia.Face;
using MagicMedia.Thumbnail;
using SixLabors.ImageSharp;

namespace Sample.Web
{
    public class SampleService
    {
        private readonly IThumbnailService _thumbnailService;
        private readonly IFaceDetectionService _faceDetectionService;
        private readonly IBoxExtractorService _faceExtractorService;

        public SampleService(
            IThumbnailService thumbnailService,
            IFaceDetectionService faceDetectionService,
            IBoxExtractorService faceExtractorService)
        {
            _thumbnailService = thumbnailService;
            _faceDetectionService = faceDetectionService;
            _faceExtractorService = faceExtractorService;
        }

        List<SampleMedia> _samples = new List<SampleMedia>();

        public void BuildSampleStore()
        {
            var root = new DirectoryInfo(GetTestMediaPath());

            foreach (FileInfo file in root.GetFiles())
            {
                var media = new SampleMedia
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Path = file.FullName,
                    Name = file.Name,
                };

                _samples.Add(media);
            }
        }

        public async Task<IEnumerable<FaceDetectionInfo>> ExtractFacesAsync(string id)
        {
            SampleMedia media = _samples.Single(x => x.Id == id);
            using var fileStream = new FileStream(media.Path, FileMode.Open);

            IEnumerable<FaceDetectionResponse> faces = await _faceDetectionService
                .DetectFacesAsync(fileStream, default);

            IList<FaceDetectionInfo> infos = faces.Select(f => new FaceDetectionInfo
            {
                Box = f.Box,
                Id = Guid.NewGuid()
            }).ToList();


            fileStream.Position = 0;
            var image = Image.Load(fileStream);

            IEnumerable<BoxExtractionInput> inputs = infos.Select(f => new BoxExtractionInput
            {
                Box = f.Box,
                Id = f.Id
            });


            IEnumerable<BoxExtractionResult> images = await _faceExtractorService
                .ExtractBoxesAsync(image, inputs, ThumbnailSizeName.M, default);

            foreach (BoxExtractionResult img in images)
            {
                FaceDetectionInfo info = infos.Single(x => x.Id == img.Id);
                info.Image = img.Thumbnail.Data.ToDataUrl("jpg");
            }

            return infos;
        }

        public async Task<IEnumerable<SampleMedia>> GetSampleMediaListAsync()
        {
            foreach (SampleMedia sample in _samples)
            {
                using var fileStream = new FileStream(sample.Path, FileMode.Open);
                ThumbnailResult thumb = await _thumbnailService
                .GenerateThumbnailAsync(
                    fileStream,
                    ThumbnailSizeName.M,
                    default);

                sample.Thumbnail = thumb.Data.ToDataUrl("jpg");
            }

            return _samples;
        }

        public byte[] GetMedia(string id)
        {
            SampleMedia media = _samples.Single(x => x.Id == id);
            using var fileStream = new FileStream(media.Path, FileMode.Open);

            return fileStream.ToByteArray();
        }

        private static string GetTestMediaPath()
        {
            string[] segments = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar);
            var idx = Array.IndexOf(segments, "samples");
            var root = string.Join(Path.DirectorySeparatorChar, segments.Take(idx));

            return Path.Join(root, "test_media");
        }
    }

    public class FaceDetectionInfo
    {
        public Guid Id { get; set; }

        public ImageBox Box { get; set; }

        public string Image { get; set; }
    }

    public class SampleMedia
    {
        public string Path { get; set; }

        public string Thumbnail { get; set; }
        public string Name { get; internal set; }
        public string Id { get; internal set; }
    }
}
