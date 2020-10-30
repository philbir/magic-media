using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.TestLibrary;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace MagicMedia.Tests.Core.FaceDetection
{
    public class FaceDetectionIntegrationTests
    {
        [Fact]
        public async Task DetectFaces()
        {
            // Arrange
            IServiceProvider sp = BuildServiceProvider();
            Stream image = TestMediaLibrary.TwoFacesNoExif;

            IFaceDetectionService faceService = sp.GetService<IFaceDetectionService>();

            // Act
            IEnumerable<FaceDetectionResponse> faces = await faceService.DetectFacesAsync(image, default);

            // Assert
            faces.MatchSnapshot();
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddFaceDetection(null);

            return services.BuildServiceProvider();
        }
    }
}
