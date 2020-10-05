using System;
using System.Collections.Generic;
using System.Text;

namespace MagicMedia
{
    public class ThumbnailSizeDefinition
    {
        public ThumbnailSizeName Name { get; set; }

        public int Width { get; set; }

        public bool IsSquare { get; set; }
    }

    public class ThumbnailResult
    {
        public byte[] Data { get; set; }

        public ThumbnailSizeName Size { get; set; }

        public MediaDimension Dimensions { get; set; }

        public string Format { get; set; }
    }

    public enum ThumbnailSizeName
    {
        Xs,
        S,
        M,
        L,
        SqXs,
        SqS
    }
}
