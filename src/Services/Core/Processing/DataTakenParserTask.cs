using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Metadata;

namespace MagicMedia.Processing
{
    public class DataTakenParserTask : IMediaProcessorTask
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
