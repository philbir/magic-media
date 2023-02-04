using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Store;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing;

public class BuildFaceDataTask : IMediaProcessorTask
{
    private readonly IFaceDetectionService _faceDetectionService;
    private readonly IBoxExtractorService _boxExtractorService;

    public BuildFaceDataTask(
        IFaceDetectionService faceDetectionService,
        IBoxExtractorService boxExtractorService)
    {
        _faceDetectionService = faceDetectionService;
        _boxExtractorService = boxExtractorService;
    }

    public string Name => MediaProcessorTaskNames.BuildFaceData;

    public async Task ExecuteAsync(
        MediaProcessorContext context,
        CancellationToken cancellationToken)
    {
        MemoryStream stream = new MemoryStream();
        await context.Image.SaveAsJpegAsync(stream, cancellationToken);
        stream.Position = 0;

        IEnumerable<FaceDetectionResponse> detectedFaces = await _faceDetectionService
            .DetectFacesAsync(stream, cancellationToken);

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

        stream.Position = 0;

        IEnumerable<BoxExtractionResult> faceImages = await _boxExtractorService
            .ExtractBoxesAsync(stream, inputs, ThumbnailSizeName.M, default);

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

        context.FaceData = faceData;
    }
}
