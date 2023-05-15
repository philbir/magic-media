using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;


public class WebPreviewImageService : IWebPreviewImageService
{
    private readonly IMediaService _mediaService;
    private readonly IWebPImageConverter _converter;

    public WebPreviewImageService(
        IMediaService mediaService,
        IWebPImageConverter converter)
    {
        _mediaService = mediaService;
        _converter = converter;
    }

    public async Task SavePreviewImageAsync(Media media, CancellationToken cancellationToken)
    {
        var filename = _mediaService.GetFilename(media, MediaFileType.Original);
        var webPFilename = _mediaService.GetFilename(media, MediaFileType.WebPreview);

        Image image = await Image.LoadAsync(filename, cancellationToken);
        image.Mutate(x => x.AutoOrient());

        try
        {
            using var ms = new MemoryStream();

            await image.SaveAsJpegAsync(ms, cancellationToken: cancellationToken);
            ms.Position = 0;
            Stream webP = _converter.ConvertToWebP(ms);
            webP.Position = 0;

            await File.WriteAllBytesAsync(webPFilename, webP.ToByteArray(), cancellationToken);

            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        await image.SaveAsync(webPFilename, new WebpEncoder() { Quality = 75 }, cancellationToken: cancellationToken);
    }
}
