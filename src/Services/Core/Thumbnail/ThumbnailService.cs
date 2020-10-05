using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia.Thumbnail
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IEnumerable<ThumbnailSizeDefinition> _sizeDefinitions;

        public ThumbnailService(IEnumerable<ThumbnailSizeDefinition> sizeDefinitions)
        {
            _sizeDefinitions = sizeDefinitions;
        }

        public async Task<IEnumerable<ThumbnailResult>> GenerateAllThumbnailAsync(
            Stream stream,
            CancellationToken cancellationToken)
        {
            Image image = await Image.LoadAsync(stream);

            return await GenerateAllThumbnailAsync(image, cancellationToken);
        }

        public async Task<IEnumerable<ThumbnailResult>> GenerateAllThumbnailAsync(
            Image image,
            CancellationToken cancellationToken)
        {
            var thumbs = new List<ThumbnailResult>();

            foreach (ThumbnailSizeDefinition? def in _sizeDefinitions)
            {
                ThumbnailResult thumb = await GenerateThumbnailAsync(
                    image,
                    def.Name,
                    cancellationToken);

                thumbs.Add(thumb);
            }

            return thumbs;
        }

        public async Task<ThumbnailResult> GenerateThumbnailAsync(
            Stream stream,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            Image image = await Image.LoadAsync(stream);
            return await GenerateThumbnailAsync(image, size, cancellationToken);
        }

        public async Task<ThumbnailResult> GenerateThumbnailAsync(
            Image image,
            ThumbnailSizeName size,
            CancellationToken cancellationToken)
        {
            ThumbnailSizeDefinition? def = _sizeDefinitions.Single(x => x.Name == size);

            if (def.IsSquare)
            {
                image = CropSqare(image);
            }

            var width = image.Width;
            var newWidth = def.Width;
            if (newWidth > width)
                newWidth = width;
            var ratio = image.Width / (double)newWidth;
            var height = image.Height / ratio;

            Image resized = image.Clone(ctx => ctx.Resize(newWidth, (int)height));
            MemoryStream thumb = new MemoryStream();
            await resized.SaveAsync(thumb, new JpegEncoder(), cancellationToken);
            thumb.Position = 0;

            return new ThumbnailResult
            {
                Size = size,
                Data = thumb.ToArray(),
                Format = "jpg",
                Dimensions = new MediaDimension()
                {
                    Height = image.Height,
                    Width = image.Width
                }
            };
        }

        private Image CropSqare(Image image)
        {
            Rectangle rect;
            if (image.GetOrientation() == MediaOrientation.Landscape)
            {
                var toRem = (image.Width - image.Height);
                rect = new Rectangle(toRem / 2, 0, image.Width - toRem, image.Height);
            }
            else
            {
                var toRem = (image.Height - image.Width);
                rect = new Rectangle(0, toRem / 2, image.Width, image.Height - toRem);
            }

            image.Mutate(x => x.Crop(rect));

            return image;
        }
    }
}
