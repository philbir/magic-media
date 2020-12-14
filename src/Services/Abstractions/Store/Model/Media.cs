using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class Media
    {
        public Guid Id { get; set; }

        public MediaType MediaType { get; set; }

        public MediaState State { get; set; }

        public string Filename { get; set; }

        public long Size { get; set; }

        public MediaSource Source { get; set; }

        public string OriginalHash { get; set; }

        public string? ImageHash { get; set; }

        public string? UniqueIdentifier { get; set; }

        public string? ImageUniqueId { get; set; }

        public DateTimeOffset? DateTaken { get; set; }

        public GeoLocation? GeoLocation { get; set; }

        public MediaDimension Dimension { get; set; }

        public int FaceCount { get; set; }

        public IEnumerable<MediaThumbnail> Thumbnails { get; set; }

        public string? Folder { get; set; }

        public bool IsFavorite { get; set; }

        public string Format { get; set; }

        public VideoInfo? VideoInfo { get; set; }

        public Guid? CameraId { get; set; }

        public MediaAISummary AISummary { get; set; }

    }

    public record MediaHeaderData(Guid Id, string Filename, DateTimeOffset? DateTaken);

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
        Validated,
        Recycled
    }

    public enum MediaState
    {
        Active,
        Recycled
    }
}
