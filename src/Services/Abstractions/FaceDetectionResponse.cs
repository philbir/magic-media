using System;
using System.Collections.Generic;
using System.Text;
using MagicMedia.Store;

namespace MagicMedia
{
    public class FaceDetectionResponse
    {
        public ImageBox Box { get; set; }
        public double[] Encoding { get; set; }
    }

    public class FaceData
    {
        public Guid Id { get; set; }

        public ImageBox Box { get; set; }

        public double[] Encoding { get; set; }

        public MediaThumbnail Thumbnail { get; set; }
    }

    public class ImageBox
    {
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
    }

    public class BoxExtractionInput
    {
        public Guid Id { get; set; }

        public ImageBox Box { get; set; }
    }
}
