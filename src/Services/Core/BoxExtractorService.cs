using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Thumbnail;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia
{
    public class BoxExtractorService : IBoxExtractorService
    {
        private readonly IThumbnailService _thumbnailService;

        public BoxExtractorService(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        public async Task<IEnumerable<BoxExtractionResult>> ExtractBoxesAsync(
            Stream stream,
            IEnumerable<BoxExtractionInput> inputs,
            ThumbnailSizeName thumbnailSize,
            CancellationToken cancellationToken)
        {
            Image image = await Image.LoadAsync(stream);

            return await ExtractBoxesAsync(image, inputs, thumbnailSize, cancellationToken);
        }

        public async Task<IEnumerable<BoxExtractionResult>> ExtractBoxesAsync(
            Image image,
            IEnumerable<BoxExtractionInput> inputs,
            ThumbnailSizeName thumbnailSize,
            CancellationToken cancellationToken)
        {
            var faces = new List<BoxExtractionResult>();

            foreach (BoxExtractionInput input in inputs)
            {
                try
                {
                    BoxExtractionResult face = await ExtractBoxAsync(
                        image,
                        input,
                        thumbnailSize,
                        cancellationToken);

                    faces.Add(face);
                }
                catch (Exception ex)
                {

                }
            }

            return faces;
        }

        private async Task<BoxExtractionResult> ExtractBoxAsync(
            Image image,
            BoxExtractionInput input,
            ThumbnailSizeName thumbnailSize,
            CancellationToken cancellationToken)
        {
            var face = new BoxExtractionResult();
            int width = input.Box.Right - input.Box.Left;
            int height = input.Box.Bottom - input.Box.Top;

            double multi = 1.95;
            var widthAdd = (int)((width * multi - width) / 2);
            //Make height and width same
            var heighAdd = Math.Abs(widthAdd + width - height);

            var faceBox = new ImageBox();

            faceBox.Left = input.Box.Left - widthAdd;
            if (faceBox.Left < 0)
            {
                faceBox.Left = 0;
            }

            faceBox.Right = input.Box.Right + widthAdd;
            if (faceBox.Right > image.Width)
            {
                faceBox.Right = image.Width;
            }

            faceBox.Top = input.Box.Top - heighAdd;
            if (faceBox.Top < 0)
            {
                faceBox.Top = 0;
            }

            faceBox.Bottom = input.Box.Bottom + heighAdd;
            if (faceBox.Bottom > image.Height)
            {
                faceBox.Bottom = image.Height;
            }

            var rect = new Rectangle(
                faceBox.Left,
                faceBox.Top,
                faceBox.Right - faceBox.Left,
                faceBox.Bottom - faceBox.Top);

            Image croped = image.Clone(x => x.Crop(rect));

            ThumbnailResult faceThumb = await _thumbnailService.GenerateThumbnailAsync(
                croped,
                thumbnailSize,
                cancellationToken);

            return new BoxExtractionResult
            {
                Id = input.Id,
                Thumbnail = faceThumb
            };
        }
    }
}
