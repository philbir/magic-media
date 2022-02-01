namespace MagicMedia.Api.Host.Tests
{
    public interface IMediaThumbnail
    {
        System.Guid Id { get; }

        ThumbnailSizeName Size { get; }

        string DataUrl { get; }

        IMediaDimension1 Dimensions { get; }
    }
}
