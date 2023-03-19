using System.IO;
using ImageMagick;

namespace MagicMedia;

public class DefaultWebPImageConverter : IWebPImageConverter
{
    public Stream ConvertToWebP(Stream stream, int quality = 50)
    {
        using var image = new MagickImage(stream);
        image.Quality = quality;
        image.Format = MagickFormat.WebP;
        var ms = new MemoryStream();

        image.Write(ms);

        return ms;
    }
}
