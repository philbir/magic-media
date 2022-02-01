namespace MagicMedia.Api.Host.Tests
{
    public class MediaFace
        : IMediaFace
    {
        public MediaFace(
            System.Guid id,
            IImageBox box,
            IMediaThumbnail1 thumbnail,
            IPerson person,
            System.Guid? personId)
        {
            Id = id;
            Box = box;
            Thumbnail = thumbnail;
            Person = person;
            PersonId = personId;
        }

        public System.Guid Id { get; }

        public IImageBox Box { get; }

        public IMediaThumbnail1 Thumbnail { get; }

        public IPerson Person { get; }

        public System.Guid? PersonId { get; }
    }
}
