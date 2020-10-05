using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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
