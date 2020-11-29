using System.IO;
using SixLabors.ImageSharp;

namespace MagicMedia.Tests.Core.Images
{
    public static class ImageStreamExtensions
    {
        public static Image AsImage(this Stream stream)
        {
            return Image.Load(stream);
        }
    }
}
