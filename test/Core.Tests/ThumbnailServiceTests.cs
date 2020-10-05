using System.Collections.Generic;
using System.Threading.Tasks;
using MagicMedia.TestLibrary;
using MagicMedia.Tests.Core.Images;
using MagicMedia.Thumbnail;
using SixLabors.ImageSharp;
using Xunit;

namespace MagicMedia.Tests.Core
{
    public class ThumbnailServiceTests
    {
        [Fact]
        public async Task GenerateThumbnail()
        {
            // Arrange
            var defs = new List<ThumbnailSizeDefinition>
            {
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.M,
                    Width = 240
                }
            };

            var service = new ThumbnailService(defs);
            Image image = TestMediaLibrary.WithExif.AsImage();

            // Act
            ThumbnailResult thumb = await service.GenerateThumbnailAsync(
                image,
                ThumbnailSizeName.M,
                default);

            // Assert
            //thumb.Dimensions.Width.Should()
        }
    }


}
