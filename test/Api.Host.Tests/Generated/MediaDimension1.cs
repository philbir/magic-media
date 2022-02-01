namespace MagicMedia.Api.Host.Tests
{
    public class MediaDimension1
        : IMediaDimension1
    {
        public MediaDimension1(
            int height,
            int width)
        {
            Height = height;
            Width = width;
        }

        public int Height { get; }

        public int Width { get; }
    }
}
