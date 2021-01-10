namespace MagicMedia
{
    public static class DownloadImageSizeMapExtensions
    {
        public static int GetMaxValue(this ImageDownloadSize size)
        {
            switch (size)
            {
                case ImageDownloadSize.Medium:
                    return 1280;
                case ImageDownloadSize.Small:
                    return 800;
                default:
                    return int.MaxValue;
            }
        }
    }
}
