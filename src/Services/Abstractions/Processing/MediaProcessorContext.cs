using System.Collections.Generic;
using MagicMedia.Discovery;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing
{
    public class MediaProcessorContext
    {
        public byte[]? OriginalData { get; set; }

        public MediaMetadata? Metadata { get; set; }

        public IEnumerable<ThumbnailResult>? Thumbnails { get; set; }

        public IEnumerable<FaceData> FaceData { get; set; }

        public Image? Image { get; set; }

        public byte[]? WebImage { get; set; }

        public MediaDiscoveryIdentifier File { get; set; }
    }
}
