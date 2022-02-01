using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface ICloudAIMediaAnalyser
{
    AISource Source { get; }

    Task<MediaAI> AnalyseImageAsync(Stream imageStream, CancellationToken cancellationToken);
}
