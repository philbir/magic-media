using System.Collections.Generic;

namespace MagicMedia.Api.Host.Tests
{
    public class Media
        : IMedia
    {
        public Media(
            System.Guid id, 
            string filename, 
            System.DateTimeOffset? dateTaken, 
            IMediaDimension dimension, 
            ICamera camera, 
            IReadOnlyList<IMediaFace> faces, 
            IMediaThumbnail thumbnail)
        {
            Id = id;
            Filename = filename;
            DateTaken = dateTaken;
            Dimension = dimension;
            Camera = camera;
            Faces = faces;
            Thumbnail = thumbnail;
        }

        public System.Guid Id { get; }

        public string Filename { get; }

        public System.DateTimeOffset? DateTaken { get; }

        public IMediaDimension Dimension { get; }

        public ICamera Camera { get; }

        public IReadOnlyList<IMediaFace> Faces { get; }

        public IMediaThumbnail Thumbnail { get; }
    }
}
