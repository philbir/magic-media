using System.IO;

namespace MagicMedia
{
    public interface IWebPImageConverter
    {
        Stream ConvertToWebP(Stream stream, int quality = 50);
    }
}