using System.Collections.Generic;
using MagicMedia.Processing;
using MagicMedia.Store;
using SixLabors.ImageSharp;

namespace MagicMedia
{
    public record AddNewMediaRequest(Media Media)
    {
        public IList<MediaFace>? Faces { get; init; }

        public SaveMediaMode SaveMode { get; init; }

        public byte[]? WebImage { get; init; }

        public Image? Image { get; init; }
    }
}
