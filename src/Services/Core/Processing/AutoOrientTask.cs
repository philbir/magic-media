using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Metadata;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing
{
    public class AutoOrientTask : IMediaProcesserTask
    {
        private readonly IImageTransformService _imageTransformService;

        public AutoOrientTask(IImageTransformService imageTransformService)
        {
            _imageTransformService = imageTransformService;
        }

        public string Name => MediaProcessorTaskNames.AutoOrient;

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {
            var stream = new MemoryStream(context.OriginalData!);
            Image image = await Image.LoadAsync(stream);

            context.Image = _imageTransformService.AutoOrient(image);
        }
    }

    public class DataTakenParserTask : IMediaProcesserTask
    {
        private readonly IDateTakenParser _dateTakenParser;

        public DataTakenParserTask(IDateTakenParser dateTakenParser)
        {
            _dateTakenParser = dateTakenParser;
        }

        public string Name => MediaProcessorTaskNames.ParseDateTaken;

        public async Task ExecuteAsync(
            MediaProcessorContext context,
            CancellationToken cancellationToken)
        {
            if (!context.Metadata.DateTaken.HasValue)
            {
                context.Metadata.DateTaken = _dateTakenParser.Parse(
                    Path.GetFileNameWithoutExtension(context.File.Id));
            }
        }
    }
}
