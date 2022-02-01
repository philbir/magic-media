using System.Threading;
using System.Threading.Tasks;

namespace MagicMedia.Video;

public interface IVideoProcessingService
{
    Task<string> ConvertTo720Async(string filename, string outfile, CancellationToken cancellationToken);
    Task<string> ConvertToWebMAsync(string filename, string outfile, CancellationToken cancellationToken);
    Task<ExtractVideoDataResult> ExtractVideoDataAsync(string filename, CancellationToken cancellationToken);
    Task<string> GeneratePreviewGifAsync(string filename, string? outfile, CancellationToken cancellationToken);
}
