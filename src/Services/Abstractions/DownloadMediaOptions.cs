namespace MagicMedia
{
    public class DownloadMediaOptions
    {
        public bool RemoveMetadata { get; set; }

        public ImageDownloadSize ImageSize { get; set; } = ImageDownloadSize.Original;

        public int? JpegCompression { get; set; }

        public VideoDownloadSize VideoSize { get; set; } = VideoDownloadSize.Original;
    }
}
