using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing;

public class GenerateWebImageTask : IMediaProcessorTask
{
    private readonly IWebPImageConverter _webPImageConverter;

    public string Name => MediaProcessorTaskNames.GenerateWebImage;

    public GenerateWebImageTask(IWebPImageConverter webPImageConverter)
    {
        _webPImageConverter = webPImageConverter;
    }

    public async Task ExecuteAsync(
        MediaProcessorContext context,
        CancellationToken cancellationToken)
    {
        MemoryStream stream = new MemoryStream();
        await context.Image.SaveAsJpegAsync(stream, cancellationToken);
        stream.Position = 0;

        context.WebImage = _webPImageConverter.ConvertToWebP(stream).ToByteArray();
    }
}
