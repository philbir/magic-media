using SixLabors.ImageSharp;

namespace MagicMedia.Extensions;

public static class ImageExtensions
{
    public static MediaOrientation GetOrientation(this Image image)
    {
        return (image.Width > image.Height) ?
            MediaOrientation.Landscape :
            MediaOrientation.Portrait;
    }
}

public static class ImageBoxExtensions
{
    public static int GetResolution(this ImageBox box)
    {
        return (box.Bottom - box.Top) * (box.Right - box.Left);
    }
}
