using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Processing;

public class SaveFaceDataAsync : IMediaProcessorTask
{
    private readonly IMediaStore _mediaStore;

    public string Name => MediaProcessorTaskNames.SaveFaces;

    public SaveFaceDataAsync(IMediaStore mediaStore)
    {
        _mediaStore = mediaStore;
    }

    public async Task ExecuteAsync(
        MediaProcessorContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<MediaFace> faces = new List<MediaFace>();

        if (context.FaceData is { })
        {
            faces = context.FaceData.Select(f => new MediaFace
            {
                Box = f.Box,
                Id = f.Id,
                Encoding = f.Encoding,
                MediaId = context.Media.Id,
                RecognitionType = f.RecognitionType,
                State = FaceState.New,
                Thumbnail = f.Thumbnail,
                PersonId = f.PersonId,
                DistanceThreshold = f.DistanceThreshold
            }).ToList();

            await _mediaStore.SaveFacesAsync(context.Media.Id, faces, cancellationToken);
        }
    }
}
