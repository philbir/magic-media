using System.IO;

namespace MagicMedia;

public interface IWebPImageConverter
{
    Stream ConvertToWebP(Stream stream, uint quality = 50);
}
