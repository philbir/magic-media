using System;

namespace MagicMedia.Store
{
    public class MediaThumbnail
    {
        public Guid Id { get; set; }

        public byte[] Data { get; set; }

        public ThumbnailSizeName Size { get; set; }

        public MediaDimension Dimensions { get; set; }

        public string Format { get; set; }

        public ThumbnailOwner Owner { get; set; }
    }


    public class ThumbnailOwner
    {
        public Guid Id { get; set; }

        public ThumbnailOwnerType Type { get; set; }
    }

    public enum ThumbnailOwnerType
    {
        Media,
        Face
    }
}
