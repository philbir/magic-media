namespace MagicMedia.Api.Host.Tests
{
    public class MediaThumbnail1
        : IMediaThumbnail1
    {
        public MediaThumbnail1(
            System.Guid id)
        {
            Id = id;
        }

        public System.Guid Id { get; }
    }
}
