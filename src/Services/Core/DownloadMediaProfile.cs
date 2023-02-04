namespace MagicMedia;

internal class DownloadMediaProfile
{
    public static DownloadMediaOptions CreateOptions(string? name)
    {
        switch (name)
        {
            case "SOCIAL_MEDIA":
                return new DownloadMediaOptions
                {
                    ImageSize = ImageDownloadSize.Medium,
                    JpegCompression = 80,
                    RemoveMetadata = true
                };
            case "IMAGE_SMALL":
                return new DownloadMediaOptions
                {
                    ImageSize = ImageDownloadSize.Small,
                    RemoveMetadata = false
                };
            default:
                return new DownloadMediaOptions
                {
                    ImageSize = ImageDownloadSize.Original,
                    VideoSize = VideoDownloadSize.Original,
                    RemoveMetadata = false
                };
        }
    }
}
