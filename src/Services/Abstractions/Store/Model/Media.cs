using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class Media
    {
        public Guid Id { get; set; }

        public MediaType MediaType { get; set; }

        public string Filename { get; set; }

        public long Size { get; set; }

        public MediaSource Source { get; set; }

        public string OriginalHash { get; set; }

        public string UniqueIdentifier { get; set; }

        public string ImageUniqueId { get; set; }

        public DateTimeOffset? DateTaken { get; set; }

        public GeoLocation GeoLocation { get; set; }

        public MediaDimension Dimension { get; set; }

        public int FaceCount { get; set; }

        public IEnumerable<MediaThumbnail> Thumbnails { get; set; }

        public string Folder { get; set; }

        public bool IsFavorite { get; set; }

        public string Format { get; set; }

        public VideoInfo VideoInfo { get; set; }

        public string CameraId { get; set; }

        public string DateTakenResolveType { get; set; }

        public int? ObjectCount { get; set; }
    }

    public enum FaceRecognitionType
    {
        None,
        Computer,
        Human
    }

    public enum FaceState
    {
        New,
        Predicted,
        Validated
    }
}
