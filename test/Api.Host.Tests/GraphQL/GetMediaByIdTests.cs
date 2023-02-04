using System;
using System.Threading.Tasks;
using FluentAssertions;
using MagicMedia.Api.Host.Tests.Infrastructure;
using StrawberryShake;
using Xunit;

namespace MagicMedia.Api.Host.Tests.GraphQL
{
    [Collection(TestCollectionNames.ApiServerWithIdentity)]
    public class GetMediaByIdTests
    {
        private readonly ApiTestServerWithIdentity _apiTestServer;

        public GetMediaByIdTests(ApiTestServerWithIdentity apiTestServer)
        {
            _apiTestServer = apiTestServer;
        }

        [Fact]
        public async Task GetMediaById_MediaExists_ReturnsExpectedMedia()
        {
            // Arrange
            Guid id = DataSeeder.DefaultMedia.Id;

            // Act
            IOperationResult<IMediaDetails> result = await _apiTestServer.GraphQLClient
                .MediaDetailsAsync(id, default);

            // Assert
            result.Data.MediaById.Filename.Should().Be("Test01.jpg");
        }
    }
}
