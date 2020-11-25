using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Processing
{
    public class CleanUpSourceTask : IMediaProcesserTask
    {
        public string Name => MediaProcessorTaskNames.CleanUpSource;

        public Task ExecuteAsync(MediaProcessorContext context, CancellationToken cancellationToken)
        {

            if (context.Options.SaveMedia.SourceAction == SaveMediaSourceAction.Delete)
            {
                File.Delete(context.File.Id);
            }
            else if (context.Options.SaveMedia.SourceAction == SaveMediaSourceAction.Move)
            {
                //TODO: Move
            }
            else if (context.Options.SaveMedia.SourceAction == SaveMediaSourceAction.Replace)
            {
                //TODO: Overwrite source
            }

            return Task.CompletedTask;
        }
    }
}
