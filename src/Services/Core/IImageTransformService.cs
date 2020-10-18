using SixLabors.ImageSharp;

namespace MagicMedia
{
    public interface IImageTransformService
    {
        Image AutoOrient(Image image);
        Image Rotate(Image image, float degrees);
    }
}