namespace MagicMedia.Api.Host.Tests
{
    public class MediaThumbnail
        : IMediaThumbnail
    {
        public MediaThumbnail(
            System.Guid id, 
            ThumbnailSizeName size, 
            string dataUrl, 
            IMediaDimension1 dimensions)
        {
            Id = id;
            Size = size;
            DataUrl = dataUrl;
            Dimensions = dimensions;
        }

        public System.Guid Id { get; }

        public ThumbnailSizeName Size { get; }

        public string DataUrl { get; }

        public IMediaDimension1 Dimensions { get; }
    }
}
