using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicMedia.Api.Host.Tests.Infrastructure;
using Xunit;

namespace MagicMedia.Api.Host.Tests
{
    [Collection(TestCollectionNames.ApiServer)]
    public class MediaEndpointTests
    {
        private readonly ApiTestServer _apiTestServer;

        public MediaEndpointTests(ApiTestServer apiTestServer)
        {
            _apiTestServer = apiTestServer;
        }


        [Fact]
        public async Task Thumbnail_ReturnsExpectedContent()
        {
            // Arrange
            Store.MediaThumbnail thumbnail = DataSeeder.DefaultMedia.Thumbnails.First();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/media/thumbnail/{thumbnail.Id}");

            // Act
            HttpResponseMessage respone = await _apiTestServer.HttpClient.SendAsync(request);
            byte[] data = await respone.Content.ReadAsByteArrayAsync();

            // Assert
            data.Should().BeEquivalentTo(thumbnail.Data);
        }

        [Fact]
        public async Task Thumbnail_ReturnsExpectedMediaType()
        {
            // Arrange
            Store.MediaThumbnail thumbnail = DataSeeder.DefaultMedia.Thumbnails.First();

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/media/thumbnail/{thumbnail.Id}");

            // Act
            HttpResponseMessage respone = await _apiTestServer.HttpClient.SendAsync(request);

            // Assert
            respone.Content.Headers.ContentType.MediaType.Should().Be("image/jpg");
        }
    }
}
