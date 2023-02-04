using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class ImageTransformService : IImageTransformService
{
    public Image AutoOrient(Image image)
    {
        Image oriented = image.Clone(x => x.AutoOrient());

        return oriented;
    }

    public Image Rotate(Image image, float degrees)
    {
        Image rotated = image.Clone(x => x.Rotate(degrees));

        return rotated;
    }
}
