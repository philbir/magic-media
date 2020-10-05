using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia;
using MagicMedia.TestLibrary;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace MagicMedia.Core.Tests
{
    public class MetadataExtractorTests
    {
        [Fact]
        public async Task GetMetadata()
        {
            // Arrange
            Mock<IGeoDecoderService> mock = CreateGeoDecoderMock();
            Stream image = TestMediaLibrary.WithExif;
            var metadataExtractor = new MetadataExtractor(mock.Object);

            // Act
            MediaMetadata meta = await metadataExtractor.GetMetadataAsync(image, default);

            // Assert
            meta.MatchSnapshot();
        }

        private Mock<IGeoDecoderService> CreateGeoDecoderMock()
        {
            var mock = new Mock<IGeoDecoderService>();
            mock
                .Setup(m => m.DecodeAsync(
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GeoAddress
                {
                    Address = "742 Evergreen Terrace",
                    City = "Springfield",
                    Country = "US",
                });

            return mock;
        }
    }
}
